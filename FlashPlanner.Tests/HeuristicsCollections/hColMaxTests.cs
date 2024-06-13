using FlashPlanner;
using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.HeuristicsCollections;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;
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
        [DataRow(5u, 1u, 2u, 3u, 5u)]
        [DataRow(2u, 1u, 2u)]
        [DataRow(uint.MaxValue, uint.MaxValue, 2u)]
        public void Can_GeneratehColMaxCorrectly(uint expected, params uint[] constants)
        {
            // ARRANGE
            IHeuristicCollection h = new hColMax(new List<IHeuristic>());
            for (int i = 0; i < constants.Length; i++)
                h.Heuristics.Add(new hConstant(constants[i]));
            var parent = new StateMove(new SASStateSpace(new TranslatorContext()));

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
