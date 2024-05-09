namespace FlashPlanner.Search
{
    public interface IHeuristicCollection : IHeuristic
    {
        public List<IHeuristic> Heuristics { get; set; }
    }
}
