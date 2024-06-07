using FlashPlanner.Heuristics;
using FlashPlanner.Models;
using PDDLSharp.Models.FastDownward.Plans;

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
        /// Get a plan for some <seealso cref="TranslatorContext"/> on the <seealso cref="IHeuristic"/> provided
        /// </summary>
        /// <param name="context"></param>
        /// <returns>A plan or null if unsolvable</returns>
        public ActionPlan? Solve(TranslatorContext context);
    }
}
