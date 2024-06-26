﻿using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.RelaxedPlanningGraphs
{
    /// <summary>
    /// Base class for generating relaxed planning graphs
    /// </summary>
    public abstract class BaseRPG
    {
        internal List<Operator> GetNewApplicableOperators(SASStateSpace state, List<Operator> operators, bool[] covered)
        {
            var result = new List<Operator>();
            for (int i = 0; i < covered.Length; i++)
            {
                if (!covered[i] && state.IsApplicable(operators[i]))
                {
                    result.Add(operators[i]);
                    covered[i] = true;
                }
            }
            return result;
        }
    }
}
