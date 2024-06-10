using FlashPlanner;
using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Heuristics;
using FlashPlanner.Models;
using FlashPlanner.Models.SAS;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Heuristics
{
    [TestClass]
    public class hDepthTests
    {
        [TestMethod]
        [DataRow(1, 1 - 1)]
        [DataRow(int.MaxValue, int.MaxValue - 1)]
        [DataRow(62362524, 62362524 - 1)]
        [DataRow(-62362524, -62362524 - 1)]
        public void Can_GeneratehDepthCorrectly(int inValue, int expected)
        {
            // ARRANGE
            IHeuristic h = new hDepth();
            var parent = new StateMove(new States.SASStateSpace(new TranslatorContext()));
            parent.hValue = inValue;

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
