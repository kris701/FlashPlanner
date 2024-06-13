using FlashPlanner;
using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;
using FlashPlanner.Tests;
using FlashPlanner.Tests.Heuristics;
using PDDLSharp;
using PDDLSharp.Models.PDDL.Domain;
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
        [DataRow(1u, 1, 1u)]
        [DataRow(1u, 2, 2u)]
        [DataRow(50u, 2, 100u)]
        [DataRow(50u, 0.5, 25u)]
        public void Can_GeneratehWeightedCorrectly(uint constant, double weight, uint expected)
        {
            // ARRANGE
            IHeuristic h = new hWeighted(new hConstant(constant), weight);
            var parent = new StateMove(new SASStateSpace(new TranslatorContext()));

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
