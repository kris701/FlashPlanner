using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.States;
using PDDLSharp.Models.FastDownward.Plans;

namespace FlashPlanner.Core.Search
{
    /// <summary>
    /// Simple <seealso href="https://en.wikipedia.org/wiki/Beam_search">Beam Search</seealso> implementation 
    /// </summary>
    public class BeamS : BaseHeuristicPlanner
    {
        /// <summary>
        /// How many states should be added to the open list pr expansion
        /// </summary>
        public int Beta { get; set; }

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        /// <param name="beta"></param>
        public BeamS(IHeuristic heuristic, int beta) : base(heuristic)
        {
            Beta = beta;
        }

        internal override ActionPlan? Solve(SASStateSpace state)
        {
            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                var newItems = new RefPriorityQueue<StateMove>();
                foreach (var opID in _context.ApplicabilityGraph[stateMove.Operator])
                {
                    var op = _context.SAS.GetOperatorByID(opID);
                    if (Abort) break;
                    if (stateMove.State.IsApplicable(op))
                    {
                        var newMove = GenerateNewState(stateMove, op);
                        if (!IsVisited(newMove) && !newItems.Contains(newMove))
                        {
                            var value = Heuristic.GetValue(stateMove, newMove.State, _context.SAS.Operators);
                            newMove.hValue = value;
                            QueueOpenList(stateMove, newMove, op);
                        }
                        if (newMove.State.IsInGoal())
                            return GeneratePlanChain(newMove);
                    }
                }
                var count = newItems.Count;
                if (count > Beta)
                    count = Beta;
                for (int i = 0; i < count; i++)
                {
                    var item = newItems.Dequeue();
                    _openList.Enqueue(item, item.hValue);
                }
            }
            return null;
        }
    }
}
