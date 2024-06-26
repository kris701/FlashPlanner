﻿using FlashPlanner.Core.Translators.Normalizers;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Translators.Grounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Translator.Normalizers
{
    [TestClass]
    public class OrNormalizerTests
    {
        public static IEnumerable<object[]> DeconstrucOrData_Valid()
        {
            yield return new object[] {
                new ActionDecl(
                    "act",
                    new ParameterExp(),
                    new PredicateExp("a"),
                    new PredicateExp("b")
                ),
                1
            };

            yield return new object[] {
                new ActionDecl(
                    "act",
                    new ParameterExp(),
                    new OrExp(new List<IExp>(){
                        new PredicateExp("a"),
                        new PredicateExp("c")
                    }),
                    new PredicateExp("b")
                ),
                2
            };

            yield return new object[] {
                new ActionDecl(
                    "act",
                    new ParameterExp(),
                    new OrExp(new List<IExp>(){
                        new OrExp(new List<IExp>(){
                            new PredicateExp("a"),
                            new PredicateExp("d")
                        }),
                        new PredicateExp("c")
                    }),
                    new PredicateExp("b")
                ),
                3
            };

            yield return new object[] {
                new ActionDecl(
                    "act",
                    new ParameterExp(),
                    new OrExp(new List<IExp>(){
                        new AndExp(new List<IExp>(){
                            new PredicateExp("a"),
                            new PredicateExp("d")
                        }),
                        new PredicateExp("c")
                    }),
                    new PredicateExp("b")
                ),
                2
            };

            yield return new object[] {
                new ActionDecl(
                    "act",
                    new ParameterExp(),
                    new OrExp(new List<IExp>(){
                        new OrExp(new List<IExp>(){
                            new PredicateExp("a"),
                            new OrExp(new List<IExp>(){
                                new PredicateExp("e"),
                                new PredicateExp("d")
                            })
                        }),
                        new PredicateExp("c")
                    }),
                    new PredicateExp("b")
                ),
                4
            };
        }

        [TestMethod]
        [DynamicData(nameof(DeconstrucOrData_Valid), DynamicDataSourceType.Method)]
        public void Can_DeconstructOr(ActionDecl input, int expectedCount)
        {
            // ARRANGE
            var deconstructor = new OrNormalizer();

            // ACT
            var result = deconstructor.DeconstructOrs(input);

            // ASSERT
            Assert.AreEqual(expectedCount, result.Count);
        }
    }
}
