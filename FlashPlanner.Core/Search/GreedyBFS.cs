using FlashPlanner.Core.Heuristics;
using PDDLSharp.Models.FastDownward.Plans;

namespace FlashPlanner.Core.Search
{
    /// <summary>
    /// Greedy Best First Search
    /// </summary>
    public class GreedyBFS : BaseHeuristicPlanner
    {
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        public GreedyBFS(IHeuristic heuristic) : base(heuristic)
        {
        }

        internal override ActionPlan? Solve()
        {   
            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                foreach (var opID in _context.ApplicabilityGraph[stateMove.Operator])
                {
                    var op = _context.SAS.GetOperatorByID(opID);
                    if (Abort) break;
                    if (stateMove.State.IsApplicable(op))
                    {
                        var newMove = GenerateNewState(stateMove, op);
                        if (!IsVisited(newMove))
                        {
                            newMove.hValue = Heuristic.GetValue(stateMove, newMove.State, _context.SAS.Operators);
                            QueueOpenList(stateMove, newMove, op);
                            if (newMove.State.IsInGoal())
                                return GeneratePlanChain(newMove);
                        }
                    }
                }
            }
            return null;
        }
    }
}
