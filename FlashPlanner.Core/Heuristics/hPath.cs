﻿using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">g-value Evaluator</seealso>
    /// </summary>
    public class hPath : BaseHeuristic
    {
        internal override int GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators) => parent.Steps + 1;
    }
}
