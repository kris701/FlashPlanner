using FlashPlanner.Models;
using FlashPlanner.Models.SAS;
using FlashPlanner.States;

namespace FlashPlanner.Heuristics
{
    /// <summary>
    /// Simply forces the search to be a depth first search
    /// </summary>
    public class hDepth : BaseHeuristic
    {
        internal override int GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            return parent.hValue - 1;
        }
    }
}
