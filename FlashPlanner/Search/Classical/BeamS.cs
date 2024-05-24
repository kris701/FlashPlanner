using FlashPlanner.Heuristics;
using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        var newMove = new StateMove(GenerateNewState(stateMove.State, op));
                        if (newMove.State.IsInGoal())
                            return new ActionPlan(GeneratePlanChain(stateMove.Steps, op));
                        if (!_closedList.Contains(newMove) && !_openList.Contains(newMove) && !newItems.Contains(newMove))
                        {
                            var value = h.GetValue(stateMove, newMove.State, Declaration.Operators);
                            newMove.Steps = new List<Operator>(stateMove.Steps) { op };
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
