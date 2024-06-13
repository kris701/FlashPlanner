using FlashPlanner.Core.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Models
{
    [TestClass]
    public class FactTests : BaseSASTests
    {
        [TestMethod]
        [DataRow(0u, 1u)]
        [DataRow(0u, 10u)]
        [DataRow(0u, 10000u)]
        public void Can_Equals_IfTrue(uint from, uint count)
        {
            // ARRANGE
            var facts1 = GenerateRandomFacts(from, count);
            var facts2 = GenerateRandomFacts(from, count);

            // ACT
            // ASSERT
            for (int i = 0; i < facts1.Count; i++)
                Assert.IsTrue(facts1[i].Equals(facts2[i]));
        }

        [TestMethod]
        [DataRow(0u, 100u, 100u)]
        [DataRow(10u, 30u, 1u)]
        [DataRow(10u, 20u, 5u)]
        public void Can_Equals_IfFalse(uint from1, uint from2, uint count)
        {
            // ARRANGE
            var facts1 = GenerateRandomFacts(from1, count);
            var facts2 = GenerateRandomFacts(from2, count);

            // ACT
            // ASSERT
            for (int i = 0; i < facts1.Count; i++)
                Assert.IsFalse(facts1[i].Equals(facts2[i]));
        }

        [TestMethod]
        public void Can_Copy()
        {
            // ARRANGE
            var facts1 = GenerateRandomFacts(0, 100);
            var facts2 = new List<Fact>();

            // ACT
            foreach (var fact in facts1)
                facts2.Add(fact.Copy());

            // ASSERT
            for (int i = 0; i < facts1.Count; i++)
                Assert.IsTrue(facts1[i].Equals(facts2[i]));
        }

        [TestMethod]
        [DataRow(1u)]
        [DataRow(100u)]
        [DataRow(1000u)]
        public void Can_GetHashCode(uint count)
        {
            // ARRANGE
            var facts = GenerateRandomFacts(0, count);

            // ACT
            // ASSERT
            Assert.AreEqual(facts.Count, facts.DistinctBy(x => x.GetHashCode()).Count());
        }
    }
}
