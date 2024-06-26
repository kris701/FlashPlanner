﻿using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.RelaxedPlanningGraphs;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.Heuristics
{
    /// <summary>
    /// Retuns the sum of actions needed to achive every goal fact
    /// </summary>
    public class hAdd : BaseHeuristic
    {
        private readonly FactRPG _graphGenerator;
        /// <summary>
        /// Main constructor
        /// </summary>
        public hAdd()
        {
            _graphGenerator = new FactRPG();
        }

        internal override uint GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            uint cost = 0;
            var dict = _graphGenerator.GenerateRelaxedGraph(state, operators);
            foreach (var fact in state.Context.SAS.Goal)
            {
                if (!dict.ContainsKey(fact.ID))
                    return int.MaxValue;
                var factCost = dict[fact.ID];
                if (factCost == int.MaxValue)
                    return int.MaxValue;
                cost += factCost;
            }
            return cost;
        }
    }
}
