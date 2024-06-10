using FlashPlanner;
using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Heuristics;
using FlashPlanner.Models;
using FlashPlanner.Models.SAS;
using FlashPlanner.RelaxedPlanningGraphs;
using PDDLSharp.Models.PDDL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Heuristics
{
    [TestClass]
    public class hConstantTests
    {
        [TestMethod]
        [DataRow(1)]
        [DataRow(int.MaxValue)]
        [DataRow(62362524)]
        [DataRow(-62362524)]
        public void Can_GeneratehConstantCorrectly(int expected)
        {
            // ARRANGE
            IHeuristic h = new hConstant(expected);
            var parent = new StateMove(new States.SASStateSpace(new TranslatorContext()));

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
