using FlashPlanner;
using FlashPlanner.Heuristics;
using FlashPlanner.HeuristicsCollections;
using FlashPlanner.Models;
using FlashPlanner.Models.SAS;
using FlashPlanner.RelaxedPlanningGraphs;
using PDDLSharp.Models.PDDL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.HeuristicsCollections
{
    [TestClass]
    public class hColMaxTests
    {
        [TestMethod]
        [DataRow(5, 1, 2, 3, 5)]
        [DataRow(2, 1, 2)]
        [DataRow(int.MaxValue, int.MaxValue, 2)]
        public void Can_GeneratehColMaxCorrectly(int expected, params int[] constants)
        {
            // ARRANGE
            IHeuristicCollection h = new hColMax(new List<IHeuristic>());
            for (int i = 0; i < constants.Length; i++)
                h.Heuristics.Add(new hConstant(constants[i]));
            var parent = new StateMove(new States.SASStateSpace(new TranslatorContext()));

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
