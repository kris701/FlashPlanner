using FlashPlanner.Heuristics;
using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search.Classical
{
    public class BeamS : BaseClassicalSearch
    {
        public int Beta { get; set; }

        public BeamS(SASDecl decl, IHeuristic heuristic, int beta) : base(decl, heuristic)
        {
            Beta = beta;
        }

        internal override ActionPlan? Solve(IHeuristic h, SASStateSpace state)
        {
            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                var newItems = new RefPriorityQueue();
                foreach (var op in Declaration.Operators)
                {
                    if (Abort) break;
                    if (stateMove.State.IsApplicable(op))
                    {
                        var newMove = GenerateNewState(stateMove, op);
                        if (newMove.State.IsInGoal())
                            return GeneratePlanChain(newMove);
                        if (!IsVisited(newMove) && !newItems.Contains(newMove))
                        {
                            var value = h.GetValue(stateMove, newMove.State, Declaration.Operators);
                            newMove.hValue = value;
                            newItems.Enqueue(newMove, value);
                        }
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
