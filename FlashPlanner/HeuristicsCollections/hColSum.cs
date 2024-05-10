﻿using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;


namespace FlashPlanner.HeuristicsCollections
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">sum Evaluator</seealso>
    /// </summary>
    public class hColSum : BaseHeuristicCollection
    {
        public hColSum() : base()
        {
        }

        public hColSum(List<IHeuristic> heuristics) : base(heuristics)
        {
        }

        public override int GetValue(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            Evaluations++;
            int sum = 0;
            foreach (var heuristic in Heuristics)
                if (sum < int.MaxValue)
                    sum = ClampSum(sum, heuristic.GetValue(parent, state, operators));
            return sum;
        }

        private int ClampSum(int value1, int value2)
        {
            if (value1 == int.MaxValue || value2 == int.MaxValue)
                return int.MaxValue;
            return value1 + value2;
        }
    }
}
