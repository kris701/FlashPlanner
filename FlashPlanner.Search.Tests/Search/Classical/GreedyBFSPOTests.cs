﻿using FlashPlanner.Search.Heuristics;
using FlashPlanner.Search.Search.Classical;
using FlashPlanner.Tests;
using FlashPlanner.Tests.Search;
using FlashPlanner.Tests.Search.Classical;
using PDDLSharp;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit;
using PDDLSharp.Toolkit.PlanValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Search.Classical
{
    [TestClass]
    public class GreedyBFSPOTests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob06.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Can_FindSolution_hDepth(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var planner = new GreedyBFSPO(decl, new hDepth());
            var validator = new PDDLSharp.Toolkit.PlanValidators.PlanValidator();

            // ACT
            var result = planner.Solve();

            // ASSERT
            Assert.IsTrue(validator.Validate(result, GetPDDLDecl(domain, problem)));
        }

        [TestMethod]
        [DataRow("TestData/gripper/domain.pddl", "TestData/gripper/prob01.pddl")]
        [DataRow("TestData/depot/domain.pddl", "TestData/depot/p01.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s1-0.pddl")]
        [DataRow("TestData/miconic/domain.pddl", "TestData/miconic/s2-4.pddl")]
        public void Can_FindSolution_hFF(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var planner = new GreedyBFSPO(decl, new hFF(decl));
            var validator = new PDDLSharp.Toolkit.PlanValidators.PlanValidator();

            // ACT
            var result = planner.Solve();

            // ASSERT
            Assert.IsTrue(validator.Validate(result, GetPDDLDecl(domain, problem)));
        }
    }
}
