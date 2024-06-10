using FlashPlanner.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Models
{
    [TestClass]
    public class BitMaskTests
    {
        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(10000)]
        public void Can_SetCorrectLength(int length)
        {
            // ARRANGE
            // ACT
            var mask = new BitMask(length);
            // ASSERT
            Assert.AreEqual(length, mask.Length);
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(40, 2)]
        [DataRow(10000, 313)]
        public void Can_SetCorrectDataLength(int length, int expected)
        {
            // ARRANGE
            // ACT
            var mask = new BitMask(length);

            // ASSERT
            Assert.AreEqual(expected, mask._dataLength);
        }

        [TestMethod]
        public void Can_SetEntireArrayToZero()
        {
            // ARRANGE
            var length = 10;

            // ACT
            var mask = new BitMask(length);

            // ASSERT
            Assert.AreEqual(length, mask.GetFalseBits());
        }

        [TestMethod]
        [DataRow(0,0)]
        [DataRow(1,0,true)]
        [DataRow(1,1,true,false)]
        [DataRow(2,1,true,false,true)]
        [DataRow(4,1,true,true,true,false,true)]
        [DataRow(4,2,true,true,true,false,true, false)]
        public void Can_SetSomeValues(int expectedTrue, int expectedFalse, params bool[] targets)
        {
            // ARRANGE
            var mask = new BitMask(targets.Length);

            // ACT
            for (int i = 0; i < targets.Length; i++)
                mask[i] = targets[i];

            // ASSERT
            Assert.AreEqual(expectedTrue, mask.GetTrueBits());
            Assert.AreEqual(expectedFalse, mask.GetFalseBits());
        }

        [TestMethod]
        [DataRow(1, 0, 0)]
        [DataRow(1, 1, 1)]
        [DataRow(1, 2, 2)]
        [DataRow(1, 200, 200)]
        [DataRow(2, 199, 200, 150)]
        public void Can_SetSomeValues_Large(int expectedTrue, int expectedFalse, params int[] targets)
        {
            // ARRANGE
            var mask = new BitMask(targets.Max() + 1);

            // ACT
            for (int i = 0; i < targets.Length; i++)
                mask[targets[i]] = true;

            // ASSERT
            Assert.AreEqual(expectedTrue, mask.GetTrueBits());
            Assert.AreEqual(expectedFalse, mask.GetFalseBits());
        }

        [TestMethod]
        public void Can_SetSingleValue()
        {
            // ARRANGE
            var length = 10;
            var target = 3;
            var mask = new BitMask(length);
            Assert.IsFalse(mask[target]);

            // ACT
            mask[target] = true;

            // ASSERT
            Assert.IsTrue(mask[target]);
        }

        [TestMethod]
        [DataRow()]
        [DataRow(true)]
        [DataRow(true, false)]
        [DataRow(true, false, true)]
        [DataRow(true, true, true, false, true)]
        [DataRow(true, true, true, false, true, false)]
        public void Can_EqualsIfTheSame(params bool[] targets)
        {
            // ARRANGE
            var mask1 = new BitMask(targets.Length);
            var mask2 = new BitMask(targets.Length);
            for(int i = 0; i < targets.Length; i++)
            {
                mask1[i] = targets[i];
                mask2[i] = targets[i];
            }

            // ACT
            // ASSERT
            Assert.AreEqual(mask1, mask2);
            Assert.IsTrue(mask1.Equals(mask2));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(true, false)]
        [DataRow(true, false, true)]
        [DataRow(true, true, true, false, true)]
        [DataRow(true, true, true, false, true, false)]
        public void Cant_EqualsIfNotTheSame(params bool[] targets)
        {
            // ARRANGE
            var mask1 = new BitMask(targets.Length);
            var mask2 = new BitMask(targets.Length);
            for (int i = 0; i < targets.Length; i++)
            {
                mask1[i] = targets[i];
                mask2[i] = !targets[i];
            }

            // ACT
            // ASSERT
            Assert.AreNotEqual(mask1, mask2);
            Assert.IsFalse(mask1.Equals(mask2));
        }

        [TestMethod]
        [DataRow(true, false, false, true, false)]
        [DataRow(true, false, true, true, false)]
        [DataRow(true, false, true, true, true)]
        [DataRow(true, true, true, true, true)]
        public void Can_SeeIfSubsetOf(params bool[] targets)
        {
            // ARRANGE
            var mask1 = new BitMask(5);
            mask1[0] = true;
            mask1[1] = false;
            mask1[2] = false;
            mask1[3] = true;
            mask1[4] = false;

            var mask2 = new BitMask(5);
            for (int i = 0; i < targets.Length; i++)
                mask2[i] = targets[i];

            // ACT
            // ASSERT
            Assert.IsTrue(mask1.IsSubsetOf(mask2));
        }

        [TestMethod]
        [DataRow(true, true, true, false, true)]
        [DataRow(true, false, false, false, true)]
        [DataRow(false, false, false, false, false)]
        public void Can_CopyOtherBitMask(params bool[] targets)
        {
            // ARRANGE
            var mask = new BitMask(targets.Length);
            for (int i = 0; i < targets.Length; i++)
                mask[i] = targets[i];

            // ACT
            var cpy = new BitMask(mask);

            // ASSERT
            Assert.AreEqual(mask, cpy);
            Assert.IsTrue(mask.Equals(cpy));
        }

        [TestMethod]
        [DataRow(2, 56)]
        [DataRow(2, 56, 512)]
        [DataRow(2, 5, 56, 512)]
        public void Can_CopyOtherBitMask_Large(params int[] targets)
        {
            // ARRANGE
            var mask = new BitMask(targets.Max() + 1);
            for (int i = 0; i < targets.Length; i++)
                mask[targets[i]] = true;

            // ACT
            var cpy = new BitMask(mask);

            // ASSERT
            Assert.AreEqual(mask, cpy);
            Assert.IsTrue(mask.Equals(cpy));
        }
    }
}
