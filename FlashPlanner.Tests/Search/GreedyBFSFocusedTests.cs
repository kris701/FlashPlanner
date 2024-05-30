using FlashPlanner;
using FlashPlanner.Heuristics;
using FlashPlanner.Search;
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
    public class GreedyBFSFocusedTests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob02.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/rovers/domain.pddl", "../../../../Dependencies/downward-benchmarks/rovers/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/zenotravel/domain.pddl", "../../../../Dependencies/downward-benchmarks/zenotravel/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/tpp/domain.pddl", "../../../../Dependencies/downward-benchmarks/tpp/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "../../../../Dependencies/downward-benchmarks/satellite/p01-pfile1.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/mystery/domain.pddl", "../../../../Dependencies/downward-benchmarks/mystery/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/driverlog/domain.pddl", "../../../../Dependencies/downward-benchmarks/driverlog/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/blocks/domain.pddl", "../../../../Dependencies/downward-benchmarks/blocks/probBLOCKS-4-0.pddl")]
        public void Can_FindSolution_hDepth(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var pddlDecl = GetPDDLDecl(domain, problem);
            var planner = new GreedyBFSFocused(new hDepth(), pddlDecl, 2, 1, 3);
            var validator = new PlanValidator();

            // ACT
            var result = planner.Solve(decl);

            // ASSERT
            Assert.IsTrue(validator.Validate(result, pddlDecl));
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob02.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/rovers/domain.pddl", "../../../../Dependencies/downward-benchmarks/rovers/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/zenotravel/domain.pddl", "../../../../Dependencies/downward-benchmarks/zenotravel/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/tpp/domain.pddl", "../../../../Dependencies/downward-benchmarks/tpp/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "../../../../Dependencies/downward-benchmarks/satellite/p01-pfile1.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/mystery/domain.pddl", "../../../../Dependencies/downward-benchmarks/mystery/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/driverlog/domain.pddl", "../../../../Dependencies/downward-benchmarks/driverlog/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/blocks/domain.pddl", "../../../../Dependencies/downward-benchmarks/blocks/probBLOCKS-4-0.pddl")]
        public void Can_FindSolution_hFF(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var pddlDecl = GetPDDLDecl(domain, problem);
            var planner = new GreedyBFSFocused(new hFF(), pddlDecl, 2, 1, 3);
            var validator = new PlanValidator();

            // ACT
            var result = planner.Solve(decl);

            // ASSERT
            Assert.IsTrue(validator.Validate(result, pddlDecl));
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob02.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/rovers/domain.pddl", "../../../../Dependencies/downward-benchmarks/rovers/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/zenotravel/domain.pddl", "../../../../Dependencies/downward-benchmarks/zenotravel/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/tpp/domain.pddl", "../../../../Dependencies/downward-benchmarks/tpp/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "../../../../Dependencies/downward-benchmarks/satellite/p01-pfile1.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/mystery/domain.pddl", "../../../../Dependencies/downward-benchmarks/mystery/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/driverlog/domain.pddl", "../../../../Dependencies/downward-benchmarks/driverlog/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/blocks/domain.pddl", "../../../../Dependencies/downward-benchmarks/blocks/probBLOCKS-4-0.pddl")]
        public void Can_FindSolution_hAdd(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var pddlDecl = GetPDDLDecl(domain, problem);
            var planner = new GreedyBFSFocused(new hAdd(), pddlDecl, 2, 1, 3);
            var validator = new PlanValidator();

            // ACT
            var result = planner.Solve(decl);

            // ASSERT
            Assert.IsTrue(validator.Validate(result, pddlDecl));
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob02.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/rovers/domain.pddl", "../../../../Dependencies/downward-benchmarks/rovers/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/zenotravel/domain.pddl", "../../../../Dependencies/downward-benchmarks/zenotravel/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/tpp/domain.pddl", "../../../../Dependencies/downward-benchmarks/tpp/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "../../../../Dependencies/downward-benchmarks/satellite/p01-pfile1.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/mystery/domain.pddl", "../../../../Dependencies/downward-benchmarks/mystery/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/driverlog/domain.pddl", "../../../../Dependencies/downward-benchmarks/driverlog/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/blocks/domain.pddl", "../../../../Dependencies/downward-benchmarks/blocks/probBLOCKS-4-0.pddl")]
        public void Can_FindSolution_hGoal(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var pddlDecl = GetPDDLDecl(domain, problem);
            var planner = new GreedyBFSFocused(new hGoal(), pddlDecl, 2, 1, 3);
            var validator = new PlanValidator();

            // ACT
            var result = planner.Solve(decl);

            // ASSERT
            Assert.IsTrue(validator.Validate(result, pddlDecl));
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob02.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/rovers/domain.pddl", "../../../../Dependencies/downward-benchmarks/rovers/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/zenotravel/domain.pddl", "../../../../Dependencies/downward-benchmarks/zenotravel/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/tpp/domain.pddl", "../../../../Dependencies/downward-benchmarks/tpp/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "../../../../Dependencies/downward-benchmarks/satellite/p01-pfile1.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/mystery/domain.pddl", "../../../../Dependencies/downward-benchmarks/mystery/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/driverlog/domain.pddl", "../../../../Dependencies/downward-benchmarks/driverlog/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/blocks/domain.pddl", "../../../../Dependencies/downward-benchmarks/blocks/probBLOCKS-4-0.pddl")]
        public void Can_FindSolution_hMax(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var pddlDecl = GetPDDLDecl(domain, problem);
            var planner = new GreedyBFSFocused(new hMax(), pddlDecl, 2, 1, 3);
            var validator = new PlanValidator();

            // ACT
            var result = planner.Solve(decl);

            // ASSERT
            Assert.IsTrue(validator.Validate(result, pddlDecl));
        }
    }
}
