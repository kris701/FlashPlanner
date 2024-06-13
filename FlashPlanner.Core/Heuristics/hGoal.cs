using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">goal count Evaluator</seealso>
    /// </summary>
    public class hGoal : BaseHeuristic
    {
        internal override uint GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            uint count = 0;
            foreach (var goal in state.Context.SAS.Goal)
                if (state[goal.ID])
                    count++;
            return (uint)state.Context.SAS.Goal.Length - count;
        }
    }
}
