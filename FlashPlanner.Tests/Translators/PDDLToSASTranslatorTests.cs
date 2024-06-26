﻿using FlashPlanner.Core;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.Translators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Toolkits;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Translator
{
    [TestClass]
    public class PDDLToSASTranslatorTests
    {
        [ClassInitialize]
        public static async Task InitialiseAsync(TestContext context)
        {
            var targetPath = "../../../../Dependencies/downward-benchmarks";
            if (!Directory.Exists(targetPath))
                throw new DirectoryNotFoundException("Benchmarks not found! Please read the readme in the Dependencies folder!");
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 34)]
        [DataRow("../../../../Dependencies/downward-benchmarks/logistics98/domain.pddl", "../../../../Dependencies/downward-benchmarks/logistics98/prob01.pddl", 360)]
        [DataRow("../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "../../../../Dependencies/downward-benchmarks/satellite/p01-pfile1.pddl", 52)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 84)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 4)]
        public void Can_Translate_ExpectedOperators(string domain, string problem, int expected)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(false);

            // ACT
            var contex = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, contex.SAS.Operators.Count);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/logistics98/domain.pddl", "../../../../Dependencies/downward-benchmarks/logistics98/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "../../../../Dependencies/downward-benchmarks/satellite/p01-pfile1.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl")]
        public void Can_Translate_ExpectedOperators_NoStatics(string domain, string problem)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(false);
            var statics = SimpleStaticPredicateDetector.FindStaticPredicates(decl);

            // ACT
            var context = translator.Translate(decl);

            // ASSERT
            foreach (var staticPred in statics)
            {
                var fact = GetFactFromPredicate(staticPred);
                foreach (var op in context.SAS.Operators)
                {
                    Assert.IsFalse(op.Pre.Any(x => x.ContentEquals(fact)));
                    Assert.IsFalse(op.Add.Any(x => x.ContentEquals(fact)));
                    Assert.IsFalse(op.Del.Any(x => x.ContentEquals(fact)));
                }
            }
        }

        private Fact GetFactFromPredicate(PredicateExp pred)
        {
            var name = pred.Name;
            var args = new List<string>();
            foreach (var arg in pred.Arguments)
                args.Add(arg.Name);
            return new Fact(name, args.ToArray());
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 4)]
        [DataRow("../../../../Dependencies/downward-benchmarks/logistics98/domain.pddl", "../../../../Dependencies/downward-benchmarks/logistics98/prob01.pddl", 6)]
        [DataRow("../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "../../../../Dependencies/downward-benchmarks/satellite/p01-pfile1.pddl", 3)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 2)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 1)]
        public void Can_Translate_ExpectedGoals(string domain, string problem, int expected)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(false);

            // ACT
            var context = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, context.SAS.Goal.Length);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 7)]
        [DataRow("../../../../Dependencies/downward-benchmarks/logistics98/domain.pddl", "../../../../Dependencies/downward-benchmarks/logistics98/prob01.pddl", 14)]
        [DataRow("../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "../../../../Dependencies/downward-benchmarks/satellite/p01-pfile1.pddl", 2)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 18)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 1)]
        public void Can_Translate_ExpectedInits(string domain, string problem, int expected)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(false);

            // ACT
            var context = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, context.SAS.Init.Length);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/pathways/domain_p01.pddl", "../../../../Dependencies/downward-benchmarks/pathways/p01.pddl", 17)]
        public void Can_Translate_ExpectedInits_NegativePreconditions(string domain, string problem, int expected)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(false);

            // ACT
            var context = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, context.SAS.Init.Length);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/pathways/domain_p01.pddl", "../../../../Dependencies/downward-benchmarks/pathways/p01.pddl", "choose")]
        public void Can_Translate_ExpectedInits_AddsNegatedFactsToOperators(string domain, string problem, params string[] ops)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(false);

            // ACT
            var context = translator.Translate(decl);

            // ASSERT
            foreach (var target in ops)
            {
                var all = context.SAS.Operators.Where(x => x.Name == target);
                foreach (var op in all)
                {
                    var negs = op.Pre.Where(x => x.Name.StartsWith("$neg-"));
                    Assert.IsTrue(negs.Count() > 0);
                    foreach (var neg in negs)
                    {
                        if (op.Add.Any(x => x.Name == neg.Name.Replace("$neg-", "")))
                            Assert.IsTrue(op.Del.Contains(neg));
                    }
                }
            }
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/logistics98/domain.pddl", "../../../../Dependencies/downward-benchmarks/logistics98/prob20.pddl")]
        public void Cant_Translate_IfTimedOut(string domain, string problem)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(false);
            translator.TimeLimit = TimeSpan.FromMilliseconds(1);

            // ACT
            var sas = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(translator.Code, ILimitedComponent.ReturnCode.TimedOut);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/rovers/domain.pddl", "../../../../Dependencies/downward-benchmarks/rovers/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/logistics00/domain.pddl", "../../../../Dependencies/downward-benchmarks/logistics00/probLOGISTICS-4-0.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/logistics98/domain.pddl", "../../../../Dependencies/downward-benchmarks/logistics98/prob01.pddl")]
        public void Cant_Translate_IfUnsolvableByRelaxedPlan(string domain, string problem)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            if (decl.Problem.Goal.GoalExp is not AndExp)
            {
                var and = new AndExp(decl.Problem.Goal, new List<IExp>());
                decl.Problem.Goal.GoalExp.Parent = and;
                and.Children.Add(decl.Problem.Goal.GoalExp);
                decl.Problem.Goal.GoalExp = and;
            }
            if (decl.Problem.Goal.GoalExp is AndExp and2)
                and2.Add(new PredicateExp("asdardfasfafk"));
            var translator = new PDDLToSASTranslator(false);

            // ACT
            var sas = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(0, sas.SAS.Operators.Count);
        }
    }
}
