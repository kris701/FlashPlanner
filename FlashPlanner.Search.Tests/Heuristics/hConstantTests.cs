﻿using FlashPlanner.Search.Heuristics;
using FlashPlanner.Search;
using FlashPlanner.Search.Tools;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Heuristics
{
    [TestClass]
    public class hConstantTests
    {
        [TestMethod]
        [DataRow(1)]
        [DataRow(int.MaxValue)]
        [DataRow(62362524)]
        [DataRow(-62362524)]
        public void Can_GeneratehConstantCorrectly(int expected)
        {
            // ARRANGE
            IHeuristic h = new hConstant(expected);
            var parent = new StateMove();

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
