﻿using FlashPlanner.Models;
using FlashPlanner.Models.SAS;
using FlashPlanner.States;

namespace FlashPlanner.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">goal count Evaluator</seealso>
    /// </summary>
    public class hGoal : BaseHeuristic
    {
        internal override int GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            int count = 0;
            foreach (var goal in state.Context.SAS.Goal)
                if (state.Contains(goal.ID))
                    count++;
            return state.Context.SAS.Goal.Length - count;
        }
    }
}