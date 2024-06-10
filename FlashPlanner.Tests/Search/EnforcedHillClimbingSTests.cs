using FlashPlanner;
using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Search;
using FlashPlanner.Tests;
using FlashPlanner.Tests.Search;
using PDDLSharp;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PlanVal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Search
{
    [TestClass]
    public class EnforcedHillClimbingSTests : BaseSearchTests
    {
        public static IEnumerable<object[]> GetData() => GetTestingData(new List<string>() {
            "gripper",
            "miconic",
            "depot",
            "rovers",
            "zenotravel",
            "tpp",
            "satellite",
            "driverlog",
            "blocks",
            "logistics00",
            "logistics98",
            "freecell",
            "movie",
            "mprime",
            "visitall-opt11-strips",
        });

        [TestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
        public void Can_FindSolution(string domain, string problem, IHeuristic h)
        {
            // ARRANGE
            var context = GetTranslatorContext(domain, problem);
            var planner = new EnforcedHillClimbingS(h);
            var validator = new PlanValidator();

            // ACT
            var result = planner.Solve(context);

            // ASSERT
            Assert.IsTrue(validator.Validate(result, context.PDDL));
        }
    }
}
