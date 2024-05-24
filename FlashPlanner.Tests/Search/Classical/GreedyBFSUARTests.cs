using FlashPlanner.Heuristics;
using FlashPlanner.Search.Classical;
using FlashPlanner.Search.Classical;
using FlashPlanner.Search.Classical;
using FlashPlanner.Tests;
using FlashPlanner.Tests.Search;
using FlashPlanner.Tests.Search.Classical;
using PDDLSharp;
using PDDLSharp.Models.PDDL.Expressions;
using PlanVal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Search.Classical
{
    [TestClass]
    public class GreedyBFSUARTests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob02.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl")]
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
            var planner = new GreedyBFSUAR(decl, new hDepth());
            var validator = new PlanValidator();

            // ACT
            var result = planner.Solve();

            // ASSERT
            Assert.IsTrue(validator.Validate(result, GetPDDLDecl(domain, problem)));
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob02.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl")]
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
            var planner = new GreedyBFSUAR(decl, new hFF());
            var validator = new PlanValidator();

            // ACT
            var result = planner.Solve();

            // ASSERT
            Assert.IsTrue(validator.Validate(result, GetPDDLDecl(domain, problem)));
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob02.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl")]
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
            var planner = new GreedyBFSUAR(decl, new hAdd());
            var validator = new PlanValidator();

            // ACT
            var result = planner.Solve();

            // ASSERT
            Assert.IsTrue(validator.Validate(result, GetPDDLDecl(domain, problem)));
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob02.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl")]
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
            var planner = new GreedyBFSUAR(decl, new hGoal());
            var validator = new PlanValidator();

            // ACT
            var result = planner.Solve();

            // ASSERT
            Assert.IsTrue(validator.Validate(result, GetPDDLDecl(domain, problem)));
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob02.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl")]
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
            var planner = new GreedyBFSUAR(decl, new hMax());
            var validator = new PlanValidator();

            // ACT
            var result = planner.Solve();

            // ASSERT
            Assert.IsTrue(validator.Validate(result, GetPDDLDecl(domain, problem)));
        }
    }
}
