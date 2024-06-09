using FlashPlanner.Models;
using FlashPlanner.Models.SAS;
using FlashPlanner.States;

namespace FlashPlanner.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">g-value Evaluator</seealso>
    /// </summary>
    public class hPath : BaseHeuristic
    {
        internal override int GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators) => parent.Steps + 1;
    }
}
