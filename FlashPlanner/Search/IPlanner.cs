using FlashPlanner.Heuristics;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search
{
    public interface IPlanner : IDisposable
    {
        public SASDecl Declaration { get; }
        public IHeuristic Heuristic { get; }

        public TimeSpan TimeLimit { get; set; }

        public int Generated { get; }
        public int Expanded { get; }
        public int Evaluations { get; }

        public bool Aborted { get; }
        public TimeSpan SearchTime { get; }

        public ActionPlan Solve();
    }
}
