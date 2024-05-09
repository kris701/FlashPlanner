﻿using FlashPlanner.Search.Tools;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;

namespace FlashPlanner.Search.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">weighted Evaluator</seealso>
    /// </summary>
    public class hWeighted : BaseHeuristic
    {
        public IHeuristic Heuristic { get; set; }
        public double Weight { get; set; }

        public hWeighted(IHeuristic heuristic, double weight)
        {
            Heuristic = heuristic;
            Weight = weight;
        }

        public override int GetValue(StateMove parent, ISASState state, List<Operator> operators)
        {
            Evaluations++;
            return (int)(Heuristic.GetValue(parent, state, operators) * Weight);
        }

        public override void Reset()
        {
            base.Reset();
            Heuristic.Reset();
        }
    }
}
