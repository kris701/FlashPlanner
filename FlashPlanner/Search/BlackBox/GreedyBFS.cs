using FlashPlanner.Heuristics;
using FlashPlanner.States;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search.BlackBox
{
    public class GreedyBFS : BaseBlackBoxSearch
    {
        public GreedyBFS(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
        }

        internal override ActionPlan? Solve(IHeuristic h, SASStateSpace state)
        {
            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                var applicables = GetApplicables(stateMove.State);
                foreach (var op in applicables)
                {
                    if (Abort) break;
                    var newMove = Simulate(stateMove, op);
                    if (newMove.State.IsInGoal())
                        return GeneratePlanChain(newMove);
                    if (!IsVisited(newMove))
                    {
                        var value = h.GetValue(stateMove, newMove.State, new List<Operator>());
                        newMove.hValue = value;
                        _openList.Enqueue(newMove, value);
                    }
                }
            }
            return null;
        }
    }
}
