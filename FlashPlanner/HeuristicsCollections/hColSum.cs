using FlashPlanner.Heuristics;
using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.HeuristicsCollections
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

        internal override int GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            int sum = 0;
            foreach (var heuristic in Heuristics)
                if (sum < int.MaxValue)
                    sum = ClampSum(sum, heuristic.GetValue(parent, state, operators));
            return sum;
        }

        private int ClampSum(int value1, int value2)
        {
            if (value1 == int.MaxValue || value2 == int.MaxValue)
                return int.MaxValue;
            return value1 + value2;
        }
    }
}
