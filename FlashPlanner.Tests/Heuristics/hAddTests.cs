using FlashPlanner.Heuristics;
using FlashPlanner.States;
using FlashPlanner.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Heuristics
{
    [TestClass]
    public class hAddTests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 8)]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob06.pddl", 28)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 8)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p05.pddl", 43)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 3)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl", 6)]
        public void Can_GeneratehAddCorrectly_FromInitialState(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var h = new hAdd();
            var state = new SASStateSpace(decl);

            // ACT
            var newValue = h.GetValue(new StateMove(), state, decl.Operators);

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 0)]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob06.pddl", 0)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 0)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p05.pddl", 0)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 0)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl", 0)]
        public void Can_GeneratehAddCorrectly_FromGoalState(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var h = new hAdd();
            var state = new SASStateSpace(decl);
            foreach (var goal in decl.Goal)
                state.Add(goal);

            // ACT
            var newValue = h.GetValue(new StateMove(), state, decl.Operators);

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
