using FlashPlanner.Heuristics;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search
{
    /// <summary>
    /// Main interface for heuristic planners
    /// </summary>
    public interface IPlanner : ILimitedComponent
    {
        /// <summary>
        /// What heuristic to use
        /// </summary>
        public IHeuristic Heuristic { get; }
        /// <summary>
        /// Amount of generated states
        /// </summary>
        public int Generated { get; }
        /// <summary>
        /// Amount of expanded states
        /// </summary>
        public int Expanded { get; }
        /// <summary>
        /// Amount of heuristic evaluations
        /// </summary>
        public int Evaluations { get; }

        /// <summary>
        /// Get a plan for some <seealso cref="SASDecl"/> on the <seealso cref="IHeuristic"/> provided
        /// </summary>
        /// <param name="decl"></param>
        /// <returns>A plan or null if unsolvable</returns>
        public ActionPlan? Solve(SASDecl decl);
    }
}
