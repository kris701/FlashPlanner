using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.RelaxedPlanningGraphs;
using FlashPlanner.Core.States;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.RelaxedPlanningGraphs
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
            var context = GetTranslatorContext(domain, problem);
            var state = new RelaxedSASStateSpace(context);
            var generator = new OperatorRPG();

            // ACT
            var result = generator.GenerateReplaxedPlan(state, context.SAS.Operators);

            // ASSERT
            Assert.IsFalse(generator.Failed);
            Assert.IsFalse(state.IsInGoal());
            state = new RelaxedSASStateSpace(state, result);
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
            var context = GetTranslatorContext(domain, problem);
            var state = new RelaxedSASStateSpace(context);
            var generator = new OperatorRPG();

            // ACT
            var result = generator.GenerateReplaxedPlan(state, context.SAS.Operators);

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
            var context = GetTranslatorContext(domain, problem);
            var state = new RelaxedSASStateSpace(context);
            var generator = new OperatorRPG();

            // ACT
            var graph = generator.GenerateRelaxedPlanningGraph(state, context.SAS.Operators);

            // ASSERT
            Assert.AreEqual(expected, graph.Count);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 9, 17, 8)]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob06.pddl", 29, 57, 28)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 6, 16, 14, 14, 34)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p05.pddl", 7, 23, 42, 88, 223, 239, 180)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 1, 2, 1, 0)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s2-4.pddl", 3, 11, 2, 0)]
        public void Can_GenerateGraph_Layer_ActionSize(string domain, string problem, params int[] expecteds)
        {
            // ARRANGE
            var context = GetTranslatorContext(domain, problem);
            var state = new RelaxedSASStateSpace(context);
            var generator = new OperatorRPG();

            // ACT
            var graph = generator.GenerateRelaxedPlanningGraph(state, context.SAS.Operators);

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
            var context = GetTranslatorContext(domain, problem);
            var state = new RelaxedSASStateSpace(context);
            var generator = new OperatorRPG();

            // ACT
            var graph = generator.GenerateRelaxedPlanningGraph(state, context.SAS.Operators);

            // ASSERT
            Assert.AreEqual(context.SAS.Init.Length, graph[0].Propositions.Count);
        }

        [TestMethod]
        public void Cant_GenerateGraph_IfNoApplicableActions_1()
        {
            // ARRANGE
            var goal = new HashSet<Fact>();
            goal.Add(new Fact("abc"));
            goal.ElementAt(0).ID = 0;
            var decl = new SASDecl(new List<Operator>(), goal.ToArray(), new Fact[0], 1);
            var state = new RelaxedSASStateSpace(new TranslatorContext(decl, new PDDLDecl(), new int[1], new Dictionary<int, List<int>>()));
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
            var goal = new HashSet<Fact>();
            goal.Add(new Fact("abc"));
            goal.ElementAt(0).ID = 0;
            var decl = new SASDecl(new List<Operator>(), goal.ToArray(), new Fact[0], 2);
            var state = new RelaxedSASStateSpace(new TranslatorContext(decl, new PDDLDecl(), new int[2], new Dictionary<int, List<int>>()));

            var actions = new List<Operator>()
            {
                new Operator(
                    "non-applicable",
                    new string[]{ "?a" },
                    new Fact[]{ new Fact(1, "wew", "?a") },
                    new Fact[]{ },
                    new Fact[]{ },
                    1)
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
            var goal = new HashSet<Fact>();
            goal.Add(new Fact("abc"));
            goal.ElementAt(0).ID = 0;
            var decl = new SASDecl(new List<Operator>(), goal.ToArray(), new Fact[0], 1);
            var state = new RelaxedSASStateSpace(new TranslatorContext(decl, new PDDLDecl(), new int[1], new Dictionary<int, List<int>>()));

            var operators = new List<Operator>()
            {
                new Operator(
                    "non-applicable",
                    new string[]{ "?a" },
                    new Fact[]{ },
                    new Fact[]{ },
                    new Fact[]{ },
                    1)
            };
            var generator = new OperatorRPG();

            // ACT
            var result = generator.GenerateRelaxedPlanningGraph(state, operators);

            // ASSERT
            Assert.AreEqual(0, result.Count);
        }
    }
}
