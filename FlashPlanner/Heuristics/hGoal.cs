using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;

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
            foreach (var goal in state.Declaration.Goal)
                if (state.Contains(goal.ID))
                    count++;
            return state.Declaration.Goal.Count - count;
        }
    }
}
