using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.HeuristicsCollections
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">sum Evaluator</seealso>
    /// </summary>
    public class hColSum : BaseHeuristicCollection
    {
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristics"></param>
        public hColSum(List<IHeuristic> heuristics) : base(heuristics)
        {
        }

        internal override uint GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            uint sum = 0;
            foreach (var heuristic in Heuristics)
                if (sum < uint.MaxValue)
                    sum = ClampSum(sum, heuristic.GetValue(parent, state, operators));
            return sum;
        }

        private uint ClampSum(uint value1, uint value2)
        {
            if (value1 == uint.MaxValue || value2 == uint.MaxValue)
                return uint.MaxValue;
            return value1 + value2;
        }
    }
}
