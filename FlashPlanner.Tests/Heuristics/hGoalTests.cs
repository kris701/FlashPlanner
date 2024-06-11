using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Heuristics
{
    [TestClass]
    public class hGoalTests
    {
        [TestMethod]
        public void Can_GeneratehGoalCorrectly_NoGoals()
        {
            // ARRANGE
            var decl = new SASDecl();
            decl.Goal = new Fact[1];
            decl.Goal[0] = new Fact("goal-fact");
            decl.Goal[0].ID = 0;
            decl.Facts = 1;
            var h = new hGoal();
            var state = new SASStateSpace(new TranslatorContext(decl, new PDDLDecl(), new int[0], new Dictionary<int, List<int>>()));
            var parent = new StateMove(state);

            // ACT
            var newValue = h.GetValue(parent, state, new List<Operator>());

            // ASSERT
            Assert.AreEqual(1, newValue);
        }

        [TestMethod]
        public void Can_GeneratehGoalCorrectly_OneGoal()
        {
            // ARRANGE
            var goals = new List<Fact>();
            goals.Add(new Fact("goal-fact-1"));
            goals.ElementAt(0).ID = 0;
            var inits = new List<Fact>();
            inits.Add(new Fact("goal-fact-1"));
            inits.ElementAt(0).ID = 0;

            var decl = new SASDecl(new List<Operator>(), goals.ToArray(), inits.ToArray(), 1);
            var h = new hGoal();
            var state = new SASStateSpace(new TranslatorContext(decl, new PDDLDecl(), new int[0], new Dictionary<int, List<int>>()));
            var parent = new StateMove(state);

            // ACT
            var newValue = h.GetValue(parent, state, new List<Operator>());

            // ASSERT
            Assert.AreEqual(0, newValue);
        }

        [TestMethod]
        public void Can_GeneratehGoalCorrectly_MultipleGoals_1()
        {
            // ARRANGE
            var goals = new List<Fact>();
            goals.Add(new Fact("goal-fact-1"));
            goals.ElementAt(0).ID = 0;
            goals.Add(new Fact("goal-fact-2"));
            goals.ElementAt(1).ID = 1;
            goals.Add(new Fact("goal-fact-3"));
            goals.ElementAt(2).ID = 2;
            var inits = new List<Fact>();
            inits.Add(new Fact("goal-fact-1"));
            inits.ElementAt(0).ID = 0;
            inits.Add(new Fact("goal-fact-2"));
            inits.ElementAt(1).ID = 1;

            var decl = new SASDecl(new List<Operator>(), goals.ToArray(), inits.ToArray(), 3);
            var h = new hGoal();
            var state = new SASStateSpace(new TranslatorContext(decl, new PDDLDecl(), new int[3], new Dictionary<int, List<int>>()));
            var parent = new StateMove(state);

            // ACT
            var newValue = h.GetValue(parent, state, new List<Operator>());

            // ASSERT
            Assert.AreEqual(1, newValue);
        }

        [TestMethod]
        public void Can_GeneratehGoalCorrectly_MultipleGoals_2()
        {
            // ARRANGE
            var goals = new List<Fact>();
            goals.Add(new Fact("goal-fact-1"));
            goals.ElementAt(0).ID = 0;
            goals.Add(new Fact("goal-fact-2"));
            goals.ElementAt(1).ID = 1;
            goals.Add(new Fact("goal-fact-3"));
            goals.ElementAt(2).ID = 2;
            var inits = new List<Fact>();
            inits.Add(new Fact("goal-fact-1"));
            inits.ElementAt(0).ID = 0;
            inits.Add(new Fact("goal-fact-2"));
            inits.ElementAt(1).ID = 1;
            inits.Add(new Fact("goal-fact-3"));
            inits.ElementAt(2).ID = 2;

            var decl = new SASDecl(new List<Operator>(), goals.ToArray(), inits.ToArray(), 3);

            var h = new hGoal();
            var state = new SASStateSpace(new TranslatorContext(decl, new PDDLDecl(), new int[0], new Dictionary<int, List<int>>()));
            var parent = new StateMove(state);

            // ACT
            var newValue = h.GetValue(parent, state, new List<Operator>());

            // ASSERT
            Assert.AreEqual(0, newValue);
        }
    }
}
