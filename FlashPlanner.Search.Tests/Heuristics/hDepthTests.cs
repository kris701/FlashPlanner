﻿using FlashPlanner.Search.Heuristics;
using FlashPlanner.Search;
using FlashPlanner.Search.Tools;
using PDDLSharp.Models;
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
    public class hDepthTests
    {
        [TestMethod]
        [DataRow(1, 1 - 1)]
        [DataRow(int.MaxValue, int.MaxValue - 1)]
        [DataRow(62362524, 62362524 - 1)]
        [DataRow(-62362524, -62362524 - 1)]
        public void Can_GeneratehDepthCorrectly(int inValue, int expected)
        {
            // ARRANGE
            IHeuristic h = new hDepth();
            var parent = new StateMove();
            parent.hValue = inValue;

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}