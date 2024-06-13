using FlashPlanner;
using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Heuristics
{
    [TestClass]
    public class hPathTests
    {
        [TestMethod]
        [DataRow(1, 1u + 1u)]
        [DataRow(20, 20u + 1u)]
        [DataRow(566, 566u + 1u)]
        public void Can_GeneratehPathCorrectly(int inValue, uint expected)
        {
            // ARRANGE
            IHeuristic h = new hPath();
            var parent = new StateMove(new SASStateSpace(new TranslatorContext()));
            parent.Steps = inValue;

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
