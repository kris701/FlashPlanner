﻿using FlashPlanner.Core.Heuristics;
using PDDLSharp.Models.FastDownward.Plans;

namespace FlashPlanner.Core.Search
{
    /// <summary>
    /// Search algorithm based on <seealso href="https://www.cs.cmu.edu/afs/cs/project/jair/pub/volume28/coles07a-html/node5.html">Enforced Hill Climbing</seealso>
    /// </summary>
    public class EnforcedHillClimbingS : BaseHeuristicPlanner
    {
        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        /// <param name="beta"></param>
        public EnforcedHillClimbingS(IHeuristic heuristic) : base(heuristic)
        {
        }

        internal override ActionPlan? Solve()
        {
            var bestH = uint.MaxValue;
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
                            _closedList.Add(newMove);
                            if (newMove.State.IsInGoal())
                            {
                                QueueOpenList(stateMove, newMove, op);
                                return GeneratePlanChain(newMove);
                            }

                            var value = Heuristic.GetValue(stateMove, newMove.State, _context.SAS.Operators);
                            newMove.hValue = value;
                            if (value < bestH)
                            {
                                _openList.Clear();
                                QueueOpenList(stateMove, newMove, op);
                                bestH = value;
                                continue;
                            }
                            QueueOpenList(stateMove, newMove, op);
                        }
                    }
                }
            }
            return null;
        }
    }
}
