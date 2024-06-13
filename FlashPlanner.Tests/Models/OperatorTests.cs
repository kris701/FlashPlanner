using FlashPlanner.Core.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Models
{
    [TestClass]
    public class OperatorTests : BaseSASTests
    {
        [TestMethod]
        public void Can_GenerateConstructorRefs()
        {
            // ARRANGE
            var pre = GenerateRandomFacts(0, 5);
            var add = GenerateRandomFacts(3, 5);
            var del = GenerateRandomFacts(10, 5);

            // ACT
            var op = new Operator("op", new string[0], pre.ToArray(), add.ToArray(), del.ToArray(), 15);

            // ASSERT
            Assert.AreEqual(pre.Count, op.Pre.Length);
            foreach (var item in pre)
            {
                Assert.IsTrue(op.Pre.Contains(item));
            }

            Assert.AreEqual(add.Count, op.Add.Length);
            foreach (var item in add)
            {
                Assert.IsTrue(op.Add.Contains(item));
            }

            Assert.AreEqual(del.Count, op.Del.Length);
            foreach (var item in del)
            {
                Assert.IsTrue(op.Del.Contains(item));
            }
        }

        [TestMethod]
        [DataRow(0u, 1u)]
        [DataRow(0u, 10u)]
        [DataRow(0u, 10000u)]
        public void Can_Equals_IfTrue(uint from, uint count)
        {
            // ARRANGE
            var ops1 = GenerateRandomOperator(from, count);
            var ops2 = GenerateRandomOperator(from, count);

            // ACT
            // ASSERT
            for (int i = 0; i < ops1.Count; i++)
                Assert.IsTrue(ops1[i].Equals(ops2[i]));
        }

        [TestMethod]
        [DataRow(0u, 100u, 100u)]
        [DataRow(10u, 30u, 1u)]
        [DataRow(10u, 20u, 5u)]
        public void Can_Equals_IfFalse(uint from1, uint from2, uint count)
        {
            // ARRANGE
            var ops1 = GenerateRandomOperator(from1, count);
            var ops2 = GenerateRandomOperator(from2, count);

            // ACT
            // ASSERT
            for (int i = 0; i < ops1.Count; i++)
                Assert.IsFalse(ops1[i].Equals(ops2[i]));
        }

        [TestMethod]
        public void Can_Copy()
        {
            // ARRANGE
            var ops1 = GenerateRandomOperator(0, 100);
            var ops2 = new List<Operator>();

            // ACT
            foreach (var op in ops1)
                ops2.Add(op.Copy());

            // ASSERT
            for (int i = 0; i < ops1.Count; i++)
                Assert.IsTrue(ops1[i].Equals(ops2[i]));
        }

        [TestMethod]
        [DataRow(1u)]
        [DataRow(100u)]
        [DataRow(1000u)]
        public void Can_GetHashCode(uint count)
        {
            // ARRANGE
            var ops1 = GenerateRandomOperator(0, count);

            // ACT
            // ASSERT
            Assert.AreEqual(ops1.Count, ops1.DistinctBy(x => x.GetHashCode()).Count());
        }
    }
}
