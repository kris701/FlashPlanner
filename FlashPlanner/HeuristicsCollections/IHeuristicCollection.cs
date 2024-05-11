using FlashPlanner.Heuristics;

namespace FlashPlanner.HeuristicsCollections
{
    public interface IHeuristicCollection : IHeuristic
    {
        public List<IHeuristic> Heuristics { get; set; }
    }
}
