namespace FlashPlanner
{
    public interface IHeuristicCollection : IHeuristic
    {
        public List<IHeuristic> Heuristics { get; set; }
    }
}
