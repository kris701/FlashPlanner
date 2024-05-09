using FlashPlanner.Search.Tools;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;

namespace FlashPlanner.Search.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">g-value Evaluator</seealso>
    /// </summary>
    public class hPath : BaseHeuristic
    {
        public override int GetValue(StateMove parent, ISASState state, List<Operator> operators)
        {
            Evaluations++;
            return parent.Steps.Count + 1;
        }
    }
}
