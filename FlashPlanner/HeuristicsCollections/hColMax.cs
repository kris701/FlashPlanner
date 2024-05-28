using FlashPlanner.Heuristics;
using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.HeuristicsCollections
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

        internal override int GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            int max = -1;
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
