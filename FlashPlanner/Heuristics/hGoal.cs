using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;

namespace FlashPlanner.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">goal count Evaluator</seealso>
    /// </summary>
    public class hGoal : BaseHeuristic
    {
        public override int GetValue(StateMove parent, ISASState state, List<Operator> operators)
        {
            Evaluations++;
            int count = 0;
            foreach (var goal in state.Declaration.Goal)
                if (state.Contains(goal))
                    count++;
            return state.Declaration.Goal.Count - count;
        }
    }
}
