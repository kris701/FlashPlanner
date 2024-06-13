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
        [DataRow(1u)]
        [DataRow(10u)]
        [DataRow(10000u)]
        public void Can_SetCorrectLength(uint length)
        {
            // ARRANGE
            // ACT
            var mask = new BitMask(length);
            // ASSERT
            Assert.AreEqual(length, mask.Length);
        }

        [TestMethod]
        [DataRow(1u, 1u)]
        [DataRow(40u, 2u)]
        [DataRow(10000u, 313u)]
        public void Can_SetCorrectDataLength(uint length, uint expected)
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
            var length = 10u;

            // ACT
            var mask = new BitMask(length);

            // ASSERT
            Assert.AreEqual(length, mask.GetFalseBits());
        }

        [TestMethod]
        [DataRow(0u, 0u)]
        [DataRow(1u, 0u, true)]
        [DataRow(1u, 1u, true, false)]
        [DataRow(2u, 1u, true, false, true)]
        [DataRow(4u, 1u, true, true, true, false, true)]
        [DataRow(4u, 2u, true, true, true, false, true, false)]
        public void Can_SetSomeValues(uint expectedTrue, uint expectedFalse, params bool[] targets)
        {
            // ARRANGE
            var mask = new BitMask((uint)targets.Length);

            // ACT
            for (uint i = 0; i < targets.Length; i++)
                mask[i] = targets[i];

            // ASSERT
            Assert.AreEqual(expectedTrue, mask.GetTrueBits());
            Assert.AreEqual(expectedFalse, mask.GetFalseBits());
        }

        [TestMethod]
        [DataRow(1u, 0u, 0u)]
        [DataRow(1u, 1u, 1u)]
        [DataRow(1u, 2u, 2u)]
        [DataRow(1u, 200u, 200u)]
        [DataRow(2u, 199u, 200u, 150u)]
        public void Can_SetSomeValues_Large(uint expectedTrue, uint expectedFalse, params uint[] targets)
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
            var length = 10u;
            var target = 3u;
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
            var mask1 = new BitMask((uint)targets.Length);
            var mask2 = new BitMask((uint)targets.Length);
            for (uint i = 0; i < targets.Length; i++)
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
            var mask1 = new BitMask((uint)targets.Length);
            var mask2 = new BitMask((uint)targets.Length);
            for (uint i = 0; i < targets.Length; i++)
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
            for (uint i = 0; i < targets.Length; i++)
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
            var mask = new BitMask((uint)targets.Length);
            for (uint i = 0; i < targets.Length; i++)
                mask[i] = targets[i];

            // ACT
            var cpy = new BitMask(mask);

            // ASSERT
            Assert.AreEqual(mask, cpy);
            Assert.IsTrue(mask.Equals(cpy));
        }

        [TestMethod]
        [DataRow(2u, 56u)]
        [DataRow(2u, 56u, 512u)]
        [DataRow(2u, 5u, 56u, 512u)]
        public void Can_CopyOtherBitMask_Large(params uint[] targets)
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
