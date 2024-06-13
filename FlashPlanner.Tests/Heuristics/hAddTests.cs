using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.States;
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
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 8u)]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob06.pddl", 28u)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 8u)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p05.pddl", 43u)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 3u)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl", 6u)]
        public void Can_GeneratehAddCorrectly_FromInitialState(string domain, string problem, uint expected)
        {
            // ARRANGE
            var context = GetTranslatorContext(domain, problem);
            var h = new hAdd();
            var state = new SASStateSpace(context);

            // ACT
            var newValue = h.GetValue(new StateMove(state), state, context.SAS.Operators);

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 0u)]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob06.pddl", 0u)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 0u)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p05.pddl", 0u)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 0u)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl", 0u)]
        public void Can_GeneratehAddCorrectly_FromGoalState(string domain, string problem, uint expected)
        {
            // ARRANGE
            var context = GetTranslatorContext(domain, problem);
            var h = new hAdd();
            var state = new SASStateSpace(context);
            foreach (var goal in context.SAS.Goal)
                state._state[goal.ID] = true;

            // ACT
            var newValue = h.GetValue(new StateMove(state), state, context.SAS.Operators);

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
