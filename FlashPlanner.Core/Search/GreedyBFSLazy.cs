﻿using FlashPlanner.Core.Heuristics;
using PDDLSharp.Models.FastDownward.Plans;

namespace FlashPlanner.Core.Search
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

        internal override ActionPlan? Solve()
        {
            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                stateMove.hValue = Heuristic.GetValue(stateMove, stateMove.State, _context.SAS.Operators);

                foreach (var opID in _context.ApplicabilityGraph[stateMove.Operator])
                {
                    var op = _context.SAS.GetOperatorByID(opID);
                    if (Abort) break;
                    if (stateMove.State.IsApplicable(op))
                    {
                        var newMove = GenerateNewState(stateMove, op);
                        if (!IsVisited(newMove))
                        {
                            newMove.hValue = stateMove.hValue;
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
