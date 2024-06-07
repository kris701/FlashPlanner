﻿using System.Runtime.CompilerServices;

namespace FlashPlanner.Models
{

    /// <summary>
    /// An implementation of 32 bit bitmasks.
    /// It is strongly inspired by the <seealso cref="System.Collections.BitArray"/> class
    /// </summary>
    public class BitMask
    {
        /// <summary>
        /// How many indexes can be stored in the bitmask
        /// </summary>
        public int Length { get; set; }

        internal int[] _data;

        /// <summary>
        /// Indexer to get and set a boolean value for a given index in the bitmask
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        /// <summary>
        /// Initialize the bitmask with a set length
        /// </summary>
        /// <param name="length"></param>
        public BitMask(int length)
        {
            Length = length;
            _data = new int[Length / 32 + 1];
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public BitMask(BitMask other)
        {
            Length = other.Length;
            _data = new int[Length / 32 + 1];
            Array.Copy(other._data, _data, _data.Length);
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

        //private int NumberOfSetBits(ulong i)
        //{
        //    i = i - ((i >> 1) & 0x5555555555555555UL);
        //    i = (i & 0x3333333333333333UL) + ((i >> 2) & 0x3333333333333333UL);
        //    return (int)(unchecked(((i + (i >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
        //}

        private int NumberOfSetBits(int i)
        {
            i = i - (i >> 1 & 0x55555555);
            i = (i & 0x33333333) + (i >> 2 & 0x33333333);
            return (i + (i >> 4) & 0x0F0F0F0F) * 0x01010101 >> 24;
        }

        /// <summary>
        /// Get the value at some index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Get(int index) => (_data[index >> 5] & 1 << index) != 0;

        /// <summary>
        /// Set the value at some index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Set(int index, bool value)
        {
            int bitMask = 1 << index;
            ref int segment = ref _data[index >> 5];

            if (value)
            {
                segment |= bitMask;
            }
            else
            {
                segment &= ~bitMask;
            }
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
                if ((_data[i] & other._data[i]) != other._data[i])
                    return false;
            return true;
        }
    }
}
