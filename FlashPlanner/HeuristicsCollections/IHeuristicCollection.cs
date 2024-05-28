using FlashPlanner.Heuristics;

namespace FlashPlanner.HeuristicsCollections
{
    /// <summary>
    /// Extension to <seealso cref="IHeuristic"/>, that contains several heuristics in it.
    /// </summary>
    public interface IHeuristicCollection : IHeuristic
    {
        /// <summary>
        /// The set of heuristics that is used.
        /// </summary>
        public List<IHeuristic> Heuristics { get; set; }
    }
}
