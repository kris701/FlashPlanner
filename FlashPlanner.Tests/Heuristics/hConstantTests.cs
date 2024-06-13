using FlashPlanner;
using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;
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
        [DataRow(1u)]
        [DataRow(uint.MaxValue)]
        [DataRow(62362524u)]
        public void Can_GeneratehConstantCorrectly(uint expected)
        {
            // ARRANGE
            IHeuristic h = new hConstant(expected);
            var parent = new StateMove(new SASStateSpace(new TranslatorContext()));

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
