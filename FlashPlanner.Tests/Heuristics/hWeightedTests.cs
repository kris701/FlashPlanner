using FlashPlanner;
using FlashPlanner.Heuristics;
using FlashPlanner.Tests;
using FlashPlanner.Tests.Heuristics;
using FlashPlanner.Tools;
using PDDLSharp;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Heuristics
{
    [TestClass]
    public class hWeightedTests
    {
        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(1, 2, 2)]
        [DataRow(50, 2, 100)]
        [DataRow(50, 0.5, 25)]
        public void Can_GeneratehWeightedCorrectly(int constant, double weight, int expected)
        {
            // ARRANGE
            IHeuristic h = new hWeighted(new hConstant(constant), weight);
            var parent = new StateMove();

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
