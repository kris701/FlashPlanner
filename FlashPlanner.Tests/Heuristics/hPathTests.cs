using FlashPlanner;
using FlashPlanner.Heuristics;
using FlashPlanner.Tools;
using PDDLSharp.Models.FastDownward.Plans;
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
    public class hPathTests
    {
        [TestMethod]
        [DataRow(1, 1 + 1)]
        [DataRow(20, 20 + 1)]
        [DataRow(566, 566 + 1)]
        public void Can_GeneratehPathCorrectly(int inValue, int expected)
        {
            // ARRANGE
            IHeuristic h = new hPath();
            var parent = new StateMove(new States.SASStateSpace(new SASDecl()));
            for (int i = 0; i < inValue; i++)
                parent.PlanSteps.Add(0);

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
