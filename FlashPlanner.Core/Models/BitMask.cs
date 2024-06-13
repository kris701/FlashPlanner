using System.Collections;

namespace FlashPlanner.Core.Models
{

    /// <summary>
    /// An implementation of 32 bit bitmasks.
    /// It is strongly inspired by the <seealso cref="System.Collections.BitArray"/> class
    /// </summary>
    public class BitMask : IEnumerable<int>
    {
        /// <summary>
        /// How many indexes can be stored in the bitmask
        /// </summary>
        public int Length;

        internal int[] _data;
        internal readonly int _dataLength;
        internal int _from;
        internal int _to;

        /// <summary>
        /// Indexer to get and set a boolean value for a given index in the bitmask
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool this[int index]
        {
            get => Get(ref index);
            set => Set(ref index, ref value);
        }

        /// <summary>
        /// Initialize the bitmask with a set length
        /// </summary>
        /// <param name="length"></param>
        public BitMask(int length)
        {
            Length = length;
            _data = new int[Length / 32 + 1];
            _dataLength = _data.Length;
            _from = -1;
            _to = length;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public BitMask(BitMask other)
        {
            Length = other.Length;
            _data = GC.AllocateUninitializedArray<int>(Length / 32 + 1);
            _dataLength = other._dataLength;
            //Buffer.BlockCopy(other._data, 0, _data, 0, _data.Length * sizeof(int));
            Array.Copy(other._data, _data, _data.Length);
            _from = other._from;
            _to = other._to;
        }

        /// <summary>
        /// Get how many bits are currently true in the bitmask
        /// </summary>
        /// <returns></returns>
        public int GetTrueBits()
        {
            var count = 0;
            foreach (int value in _data)
                count += NumberOfSetBits(value);
            return count;
        }

        /// <summary>
        /// Get how many bits are currently false in the bitmask
        /// </summary>
        /// <returns></returns>
        public int GetFalseBits() => Length - GetTrueBits();

        private int NumberOfSetBits(int i)
        {
            i = i - (i >> 1 & 0x55555555);
            i = (i & 0x33333333) + (i >> 2 & 0x33333333);
            return (i + (i >> 4) & 0x0F0F0F0F) * 0x01010101 >> 24;
        }

        private bool Get(ref int index) => (_data[index >> 5] & 1 << index) != 0;

        private void Set(ref int index, ref bool value)
        {
            int bitMask = 1 << index;
            ref int segment = ref _data[index >> 5];

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
                for (int i = 0; i < _data.Length; i++)
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
            for (int i = 0; i < _data.Length; i++)
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
            for (int i = 0; i < _data.Length; i++)
                _data[i] |= other._data[i];
        }

        /// <summary>
        /// Sets bits from another bitmask to false in the current bitmask
        /// </summary>
        /// <param name="other"></param>
        public void NAnd(BitMask other)
        {
            for (int i = 0; i < _data.Length; i++)
                _data[i] &= ~other._data[i];
        }

        /// <summary>
        /// Generate bounds for iteration.
        /// Do note, this is a "static" thing and does not update itself when changes are made to this bitmask!
        /// </summary>
        public void GenerateBounds()
        {
            for(int i = 0; i < Length; i++)
            {
                if (this[i])
                {
                    _from = i - 1;
                    break;
                }
            }
            for(int i = Length; i >= _from + 1; i--)
            {
                if (this[i])
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

        public IEnumerator<int> GetEnumerator() => new BitMaskEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new BitMaskEnumerator(this);
    }

    public class BitMaskEnumerator : IEnumerator<int>
    {
        private readonly BitMask _mask;
        private int _position = -1;

        public BitMaskEnumerator(BitMask mask)
        {
            _mask = mask;
            _position = mask._from;
        }

        public bool MoveNext()
        {
            do
            {
                _position++;
                if (_position >= _mask._to)
                    return false;
            }
            while (!_mask[_position]);
            return (_position < _mask._to);
        }

        public void Reset()
        {
            _position = _mask._from;
        }

        object IEnumerator.Current => Current;

        public int Current => _position;

        public void Dispose()
        {
        }
    }
}
