﻿using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Heuristics
{
    /// <summary>
    /// Main hFF implementation based on relaxed planning graphs
    /// </summary>
    public class hFF : BaseHeuristic
    {
        private readonly OperatorRPG _graphGenerator;

        /// <summary>
        /// Main constructor
        /// </summary>
        public hFF()
        {
            _graphGenerator = new OperatorRPG();
        }

        internal override int GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            var relaxedPlan = _graphGenerator.GenerateReplaxedPlan(
                state,
                operators);
            if (_graphGenerator.Failed)
                return int.MaxValue;
            return relaxedPlan.Count;
        }
    }
}
