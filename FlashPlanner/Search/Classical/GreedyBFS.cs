﻿using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search.Classical
{
    public class GreedyBFS : BaseClassicalSearch
    {
        public GreedyBFS(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
        }

        internal override ActionPlan? Solve(IHeuristic h, SASStateSpace state)
        {
            while (!Aborted && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                foreach (var op in Declaration.Operators)
                {
                    if (Aborted) break;
                    if (stateMove.State.IsApplicable(op))
                    {
                        var newMove = new StateMove(GenerateNewState(stateMove.State, op));
                        if (newMove.State.IsInGoal())
                            return new ActionPlan(GeneratePlanChain(stateMove.Steps, op));
                        if (!_closedList.Contains(newMove) && !_openList.Contains(newMove))
                        {
                            var value = h.GetValue(stateMove, newMove.State, Declaration.Operators);
                            newMove.Steps = new List<Operator>(stateMove.Steps) { op };
                            newMove.hValue = value;
                            _openList.Enqueue(newMove, value);
                        }
                    }
                }
            }
            return null;
        }
    }
}
