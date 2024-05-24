﻿using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.SAS;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Tools
{
    [TestClass]
    public class OperatorRPGTests : BasePlannerTests
    {
        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob06.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p05.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl")]
        public void Can_GenerateRelaxedPlan_ResultsInGoal(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var state = new RelaxedSASStateSpace(decl);
            var generator = new OperatorRPG();

            // ACT
            var result = generator.GenerateReplaxedPlan(state, decl.Operators);

            // ASSERT
            Assert.IsFalse(generator.Failed);
            Assert.IsFalse(state.IsInGoal());
            foreach (var item in result)
                state.Execute(item);
            Assert.IsTrue(state.IsInGoal());
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 9)]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob06.pddl", 29)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 10)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p05.pddl", 37)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 3)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl", 6)]
        public void Can_GenerateRelaxedPlan_Length(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var state = new RelaxedSASStateSpace(decl);
            var generator = new OperatorRPG();

            // ACT
            var result = generator.GenerateReplaxedPlan(state, decl.Operators);

            // ASSERT
            Assert.IsFalse(generator.Failed);
            Assert.AreEqual(expected, result.Count);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 3)]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob06.pddl", 3)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 5)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p05.pddl", 7)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 4)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl", 4)]
        public void Can_GenerateGraph_Layer_Size(string domain, string problem, int expected)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var state = new RelaxedSASStateSpace(decl);
            var generator = new OperatorRPG();

            // ACT
            var graph = generator.GenerateRelaxedPlanningGraph(state, decl.Operators);

            // ASSERT
            Assert.AreEqual(expected, graph.Count);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 10, 28, 36)]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob06.pddl", 30, 88, 116)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 8, 28, 42, 56, 90)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p05.pddl", 9, 36, 78, 166, 389, 628, 808)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 1, 3, 4, 4)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl", 3, 14, 16, 16)]
        public void Can_GenerateGraph_Layer_ActionSize(string domain, string problem, params int[] expecteds)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var state = new RelaxedSASStateSpace(decl);
            var generator = new OperatorRPG();

            // ACT
            var graph = generator.GenerateRelaxedPlanningGraph(state, decl.Operators);

            // ASSERT
            Assert.AreEqual(expecteds.Length, graph.Count);
            for (int i = 0; i < expecteds.Length; i++)
                Assert.AreEqual(expecteds[i], graph[i].Operators.Count);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob06.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p05.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl")]
        public void Can_GenerateGraph_Layer_Proposition_FirstAlwaysInits(string domain, string problem)
        {
            // ARRANGE
            var decl = GetSASDecl(domain, problem);
            var state = new RelaxedSASStateSpace(decl);
            var generator = new OperatorRPG();

            // ACT
            var graph = generator.GenerateRelaxedPlanningGraph(state, decl.Operators);

            // ASSERT
            Assert.AreEqual(decl.Init.Count, graph[0].Propositions.Count);
        }

        [TestMethod]
        public void Cant_GenerateGraph_IfNoApplicableActions_1()
        {
            // ARRANGE
            var decl = new SASDecl();
            decl.Goal.Add(new Fact("abc"));
            var state = new RelaxedSASStateSpace(decl);
            var generator = new OperatorRPG();

            // ACT
            var result = generator.GenerateRelaxedPlanningGraph(state, new List<Operator>());

            // ASSERT
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Cant_GenerateGraph_IfNoApplicableActions_2()
        {
            // ARRANGE
            var decl = new SASDecl();
            decl.Goal.Add(new Fact("abc"));
            var state = new RelaxedSASStateSpace(decl);

            var actions = new List<Operator>()
            {
                new Operator(
                    "non-applicable",
                    new string[]{ "?a" },
                    new Fact[]{ new Fact("wew", "?a") },
                    new Fact[]{ },
                    new Fact[]{ })
            };
            var generator = new OperatorRPG();

            // ACT
            var result = generator.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Cant_GenerateGraph_IfActionDoesNothing()
        {
            // ARRANGE
            var decl = new SASDecl();
            decl.Goal.Add(new Fact("abc"));
            var state = new RelaxedSASStateSpace(decl);

            var actions = new List<Operator>()
            {
                new Operator(
                    "non-applicable",
                    new string[]{ "?a" },
                    new Fact[]{ },
                    new Fact[]{ },
                    new Fact[]{ })
            };
            var generator = new OperatorRPG();

            // ACT
            var result = generator.GenerateRelaxedPlanningGraph(state, actions);

            // ASSERT
            Assert.AreEqual(0, result.Count);
        }
    }
}
