﻿using FlashPlanner.Heuristics;
using FlashPlanner.States;
using PDDLSharp.Models.FastDownward.Plans;

namespace FlashPlanner.Search
{
    public class GreedyBFS : BaseHeuristicPlanner
    {
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        public GreedyBFS(IHeuristic heuristic) : base(heuristic)
        {
        }

        internal override ActionPlan? Solve(SASStateSpace state)
        {
            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
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
                            var value = Heuristic.GetValue(stateMove, newMove.State, _declaration.Operators);
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
