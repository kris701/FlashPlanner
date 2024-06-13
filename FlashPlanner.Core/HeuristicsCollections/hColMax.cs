using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.HeuristicsCollections
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">max Evaluator</seealso>
    /// </summary>
    public class hColMax : BaseHeuristicCollection
    {
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristics"></param>
        public hColMax(List<IHeuristic> heuristics) : base(heuristics)
        {
        }

        internal override uint GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            uint max = 0;
            foreach (var heuristic in Heuristics)
            {
                var hValue = heuristic.GetValue(parent, state, operators);
                if (hValue > max)
                    max = hValue;
            }
            return max;
        }
    }
}
