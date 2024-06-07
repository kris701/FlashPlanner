using FlashPlanner.Translators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.SAS;
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
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/logistics98/domain.pddl", "../../../../Dependencies/downward-benchmarks/logistics98/prob01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "../../../../Dependencies/downward-benchmarks/satellite/p01-pfile1.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl")]
        public void Can_Translate_ExpectedDomainVars(string domain, string problem)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(true, false);
            int expected = 0;
            if (decl.Domain.Constants != null)
                expected += decl.Domain.Constants.Constants.Count;
            if (decl.Problem.Objects != null)
                expected += decl.Problem.Objects.Objs.Count;

            // ACT
            var context = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, context.SAS.DomainVariables.Count);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob01.pddl", 36)]
        [DataRow("../../../../Dependencies/downward-benchmarks/logistics98/domain.pddl", "../../../../Dependencies/downward-benchmarks/logistics98/prob01.pddl", 384)]
        [DataRow("../../../../Dependencies/downward-benchmarks/satellite/domain.pddl", "../../../../Dependencies/downward-benchmarks/satellite/p01-pfile1.pddl", 59)]
        [DataRow("../../../../Dependencies/downward-benchmarks/depot/domain.pddl", "../../../../Dependencies/downward-benchmarks/depot/p01.pddl", 90)]
        [DataRow("../../../../Dependencies/downward-benchmarks/miconic/domain.pddl", "../../../../Dependencies/downward-benchmarks/miconic/s1-0.pddl", 4)]
        public void Can_Translate_ExpectedOperators(string domain, string problem, int expected)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(true, false);

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
            var translator = new PDDLToSASTranslator(true, false);
            var statics = SimpleStaticPredicateDetector.FindStaticPredicates(decl);

            // ACT
            var context = translator.Translate(decl);

            // ASSERT
            foreach (var staticPred in statics)
            {
                var fact = GetFactFromPredicate(staticPred);
                foreach (var op in context.SAS.Operators)
                {
                    Assert.IsFalse(op.Pre.Contains(fact));
                    Assert.IsFalse(op.Add.Contains(fact));
                    Assert.IsFalse(op.Del.Contains(fact));
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
            var translator = new PDDLToSASTranslator(true, false);

            // ACT
            var context = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, context.SAS.Goal.Count);
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
            var translator = new PDDLToSASTranslator(true, false);

            // ACT
            var context = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, context.SAS.Init.Count);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/pathways/domain_p01.pddl", "../../../../Dependencies/downward-benchmarks/pathways/p01.pddl", 17)]
        public void Can_Translate_ExpectedInits_NegativePreconditions(string domain, string problem, int expected)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(true, false);

            // ACT
            var context = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(expected, context.SAS.Init.Count);
        }

        [TestMethod]
        [DataRow("../../../../Dependencies/downward-benchmarks/pathways/domain_p01.pddl", "../../../../Dependencies/downward-benchmarks/pathways/p01.pddl", "choose")]
        public void Can_Translate_ExpectedInits_AddsNegatedFactsToOperators(string domain, string problem, params string[] ops)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(true, false);

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
        [DataRow("../../../../Dependencies/downward-benchmarks/gripper/domain.pddl", "../../../../Dependencies/downward-benchmarks/gripper/prob20.pddl")]
        [DataRow("../../../../Dependencies/downward-benchmarks/logistics98/domain.pddl", "../../../../Dependencies/downward-benchmarks/logistics98/prob20.pddl")]
        public void Cant_Translate_IfTimedOut(string domain, string problem)
        {
            // ARRANGE
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(domain), new FileInfo(problem));
            var translator = new PDDLToSASTranslator(true, false);
            translator.TimeLimit = TimeSpan.FromMilliseconds(1);

            // ACT
            var sas = translator.Translate(decl);

            // ASSERT
            Assert.AreEqual(translator.Code, ILimitedComponent.ReturnCode.TimedOut);
        }
    }
}
