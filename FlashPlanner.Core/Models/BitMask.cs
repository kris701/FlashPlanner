using System.Buffers;
using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace FlashPlanner.Core.Models
{

    /// <summary>
    /// An implementation of 32 bit bitmasks.
    /// It is strongly inspired by the <seealso cref="System.Collections.BitArray"/> class
    /// </summary>
    public class BitMask : IEnumerable<uint>
    {
        /// <summary>
        /// How many indexes can be stored in the bitmask
        /// </summary>
        public readonly uint Length;

        internal readonly uint[] _data;
        internal readonly uint _dataLength;
        internal uint _from;
        internal uint _to;

        /// <summary>
        /// Indexer to get and set a boolean value for a given index in the bitmask
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool this[uint index]
        {
            get => Get(ref index);
            set => Set(ref index, ref value);
        }

        /// <summary>
        /// Initialize the bitmask with a set length
        /// </summary>
        /// <param name="length"></param>
        public BitMask(uint length)
        {
            Length = length;
            _dataLength = Length / 32 + 1;
            _data = new uint[_dataLength];
            _from = 0;
            _to = _dataLength;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public BitMask(BitMask other)
        {
            Length = other.Length;
            _dataLength = other._dataLength;
            _data = new uint[_dataLength];
            other._data.AsSpan().CopyTo(_data);
            //_data = GC.AllocateUninitializedArray<int>(Length / 32 + 1, true);
            //Buffer.BlockCopy(other._data, 0, _data, 0, _data.Length * sizeof(int));
            //Array.Copy(other._data, _data, _data.Length);
            _from = other._from;
            _to = other._to;
        }

        /// <summary>
        /// Get how many bits are currently true in the bitmask
        /// </summary>
        /// <returns></returns>
        public uint GetTrueBits()
        {
            var count = 0u;
            for (uint i = 0; i < _data.Length; i++)
                count += (uint)BitOperations.PopCount(_data[i]);
            return count;
        }

        /// <summary>
        /// Get how many bits are currently false in the bitmask
        /// </summary>
        /// <returns></returns>
        public uint GetFalseBits() => Length - GetTrueBits();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Get(ref uint index) => (_data[index >> 5] & (1u << (int)index)) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Set(ref uint index, ref bool value)
        {
            uint bitMask = 1u << (int)index;
            ref uint segment = ref _data[index >> 5];

            if (value)
                segment |= bitMask;
            else
                segment &= ~bitMask;
        }

        /// <summary>
        /// Checks if another bitmask contains the same values as the current one
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is BitMask other)
            {
                if (other.Length != Length) return false;
                for (uint i = 0; i < _dataLength; i++)
                    if (other._data[i] != _data[i])
                        return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Simple hashcode override
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => HashCode.Combine(_data);

        /// <summary>
        /// Returns true, if the current bitmask is a subset of another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsSubsetOf(BitMask other)
        {
            for (uint i = 0; i < _dataLength; i++)
                if ((_data[i] & other._data[i]) != _data[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Apply another bitmask on the current one with or
        /// </summary>
        /// <param name="other"></param>
        public void Or(BitMask other)
        {
            for (uint i = 0; i < _dataLength; i++)
                _data[i] |= other._data[i];
        }

        /// <summary>
        /// Sets bits from another bitmask to false in the current bitmask
        /// </summary>
        /// <param name="other"></param>
        public void NAnd(BitMask other)
        {
            for (uint i = 0; i < _dataLength; i++)
                _data[i] &= ~other._data[i];
        }

        /// <summary>
        /// Generate bounds for iteration.
        /// Do note, this is a "static" thing and does not update itself when changes are made to this bitmask!
        /// </summary>
        public void GenerateBounds()
        {
            for (uint i = 0; i < _dataLength; i++)
            {
                if (_data[i] != 0)
                {
                    _from = i;
                    break;
                }
            }
            for (uint i = _dataLength - 1; i >= _from + 1; i--)
            {
                if (_data[i] != 0)
                {
                    _to = i + 1;
                    break;
                }
            }
        }

        public override string ToString()
        {
            var str = $"Facts ({GetTrueBits()}): ";
            foreach (var fact in this)
                str += $"{fact}, ";
            return str;
        }

        public IEnumerator<uint> GetEnumerator() => new BitMaskEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new BitMaskEnumerator(this);
    }

    public class BitMaskEnumerator : IEnumerator<uint>
    {
        private readonly BitMask _mask;
        private uint _maskOffset = 0;
        private uint _tmpOffset = 32;
        private uint _tmpMax = 0;
        private uint[] _tmp;

        public BitMaskEnumerator(BitMask mask)
        {
            _mask = mask;
            _maskOffset = mask._from;
            _tmp = new uint[32];
        }

        public bool MoveNext()
        {
            _tmpOffset++;
            while (_tmpOffset >= _tmpMax)
            {
                if (_maskOffset >= _mask._to)
                    return false;
                GetExponents(_mask._data[_maskOffset], _maskOffset++ * 32, ref _tmp);
                if (_tmpMax > 0)
                {
                    _tmpOffset = 0;
                    break;
                }
            }

            return true;
        }

        static uint[] MulDeBruijnBitPos = new uint[32]
        {
          0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
          31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
        };

        private void GetExponents(uint value, uint offset, ref uint[] data)
        {
            uint enabledBitCounter = 0;

            while (value != 0)
            {
                uint m = (value & (0 - value));
                value ^= m;
                data[enabledBitCounter++] = MulDeBruijnBitPos[(m * 0x077CB531U) >> 27] + offset;
            }

            _tmpMax = enabledBitCounter;
        }

        public void Reset()
        {
        }

        object IEnumerator.Current => Current;

        public uint Current => _tmp[_tmpOffset];

        public void Dispose()
        {
        }
    }
}
