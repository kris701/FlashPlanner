using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.Heuristics
{
    /// <summary>
    /// Base abstract implementation of a <seealso cref="IHeuristic"/>
    /// </summary>
    public abstract class BaseHeuristic : IHeuristic
    {
        /// <summary>
        /// How many evaluations this heuristic have made so far.
        /// </summary>
        public int Evaluations { get; private set; }
        /// <summary>
        /// Get a heuristic value for the current <paramref name="state"/>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="state"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        public uint GetValue(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            Evaluations++;
            return GetValueInner(parent, state, operators);
        }
        internal abstract uint GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators);
        /// <summary>
        /// Reset the heuristic.
        /// </summary>
        public void Reset()
        {
            Evaluations = 0;
        }
    }
}
