using FlashPlanner.Heuristics;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search
{
    public interface IPlanner : IDisposable, ILimitedComponent
    {
        public SASDecl Declaration { get; }
        public IHeuristic Heuristic { get; }

        public int Generated { get; }
        public int Expanded { get; }
        public int Evaluations { get; }

        public ActionPlan Solve();
    }
}
