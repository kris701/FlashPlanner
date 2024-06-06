using FlashPlanner.Heuristics;
using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.FastDownward.Plans;

namespace FlashPlanner.Search
{
    /// <summary>
    /// Lazy Greedy Best First Search based on the 
    /// <seealso href="https://www.fast-downward.org/Doc/SearchAlgorithm#Lazy_best-first_search">Fast Downward</seealso> implementation.
    /// </summary>
    public class GreedyBFSLazy : BaseHeuristicPlanner
    {
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        public GreedyBFSLazy(IHeuristic heuristic) : base(heuristic)
        {
        }

        internal override ActionPlan? Solve(SASStateSpace state)
        {
            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                stateMove.hValue = Heuristic.GetValue(stateMove, stateMove.State, _declaration.Operators);

                foreach (var op in _declaration.Operators)
                {
                    if (Abort) break;
                    if (stateMove.State.IsApplicable(op))
                    {
                        var newMove = GenerateNewState(stateMove, op);
                        if (newMove.State.IsInGoal())
                            return GeneratePlanChain(newMove);
                        if (!IsVisited(newMove))
                        {
                            newMove.hValue = stateMove.hValue;
                            _openList.Enqueue(newMove, stateMove.hValue);
                        }
                    }
                }
            }
            return null;
        }
    }
}
