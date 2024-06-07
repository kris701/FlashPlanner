using FlashPlanner.Heuristics;
using FlashPlanner.Models;
using FlashPlanner.States;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.HeuristicsCollections
{
    /// <summary>
    /// Main abstract implementation of collection heuristics
    /// </summary>
    public abstract class BaseHeuristicCollection : IHeuristicCollection
    {
        /// <summary>
        /// How many evaluations this heuristic have made so far.
        /// </summary>
        public int Evaluations { get; private set; }
        /// <summary>
        /// The set of heuristics that is used.
        /// </summary>
        public List<IHeuristic> Heuristics { get; set; }

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristics"></param>
        public BaseHeuristicCollection(List<IHeuristic> heuristics)
        {
            Heuristics = heuristics;
        }

        /// <summary>
        /// Get a heuristic value for the current <paramref name="state"/>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="state"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        public int GetValue(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            Evaluations++;
            return GetValueInner(parent, state, operators);
        }

        internal abstract int GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators);
        /// <summary>
        /// Reset the heuristic.
        /// </summary>
        public void Reset()
        {
            Evaluations = 0;
        }
    }
}
