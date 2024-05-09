﻿using FlashPlanner.Search.Heuristics;
using FlashPlanner.Search.HeuristicsCollections;
using FlashPlanner.Search;
using FlashPlanner.Search.Tools;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.HeuristicsCollections
{
    [TestClass]
    public class hColSumTests
    {
        [TestMethod]
        [DataRow(11, 1, 2, 3, 5)]
        [DataRow(3, 1, 2)]
        public void Can_GeneratehColSumCorrectly(int expected, params int[] constants)
        {
            // ARRANGE
            IHeuristicCollection h = new hColSum();
            for (int i = 0; i < constants.Length; i++)
                h.Heuristics.Add(new hConstant(constants[i]));
            var parent = new StateMove();

            // ACT
            var newValue = h.GetValue(parent, null, new List<Operator>());

            // ASSERT
            Assert.AreEqual(expected, newValue);
        }
    }
}
