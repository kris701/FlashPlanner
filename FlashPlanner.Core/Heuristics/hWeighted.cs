﻿using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">weighted Evaluator</seealso>
    /// It simply takes a heuristic, and multiplies its value by some constant.
    /// </summary>
    public class hWeighted : BaseHeuristic
    {
        /// <summary>
        /// The child heuristic
        /// </summary>
        public IHeuristic Heuristic { get; set; }
        /// <summary>
        /// The weight to multiply with
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        /// <param name="weight"></param>
        public hWeighted(IHeuristic heuristic, double weight)
        {
            Heuristic = heuristic;
            Weight = weight;
        }

        /// <summary>
        /// Get a heuristic value for the current <paramref name="state"/>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="state"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        internal override uint GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators) => (uint)(Heuristic.GetValue(parent, state, operators) * Weight);
    }
}
