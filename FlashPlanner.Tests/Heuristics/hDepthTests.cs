using FlashPlanner;
using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;
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
        [DataRow(1u, 1u - 1u)]
        [DataRow(uint.MaxValue, uint.MaxValue - 1)]
        [DataRow(62362524u, 62362524u - 1u)]
        public void Can_GeneratehDepthCorrectly(uint inValue, uint expected)
        {
            // ARRANGE
            IHeuristic h = new hDepth();
            var parent = new StateMove(new SASStateSpace(new TranslatorContext()));
            parent.hValue = inValue;

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
