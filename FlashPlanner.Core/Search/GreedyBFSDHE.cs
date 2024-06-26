﻿using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models;
using PDDLSharp.Models.FastDownward.Plans;

namespace FlashPlanner.Core.Search
{
    /// <summary>
    /// Greedy Best First Search with Deferred Heuristic Evaluation
    /// (<seealso href="https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf">Helmert 2006</seealso>)
    /// </summary>
    public class GreedyBFSDHE : BaseHeuristicPlanner
    {
        private readonly Dictionary<StateMove, bool> _isEvaluated = new Dictionary<StateMove, bool>();

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        public GreedyBFSDHE(IHeuristic heuristic) : base(heuristic)
        {
        }

        internal override ActionPlan? Solve()
        {
            _isEvaluated.Clear();

            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                if (_isEvaluated.TryGetValue(stateMove, out bool isEvaluated) && !isEvaluated)
                    stateMove.hValue = Heuristic.GetValue(stateMove, stateMove.State, _context.SAS.Operators);

                bool lowerFound = false;
                foreach (var opID in _context.ApplicabilityGraph[stateMove.Operator])
                {
                    var op = _context.SAS.GetOperatorByID(opID);
                    if (Abort) break;
                    if (stateMove.State.IsApplicable(op))
                    {
                        var newMove = GenerateNewState(stateMove, op);
                        if (!IsVisited(newMove))
                        {
                            if (lowerFound)
                            {
                                newMove.hValue = stateMove.hValue;
                                _isEvaluated.Add(newMove, false);
                                QueueOpenList(stateMove, newMove, op);
                                if (newMove.State.IsInGoal())
                                    return GeneratePlanChain(newMove);
                            }
                            else
                            {
                                var value = Heuristic.GetValue(stateMove, newMove.State, _context.SAS.Operators);
                                if (value < stateMove.hValue)
                                    lowerFound = true;
                                newMove.hValue = value;
                                _isEvaluated.Add(newMove, true);
                                QueueOpenList(stateMove, newMove, op);
                                if (newMove.State.IsInGoal())
                                    return GeneratePlanChain(newMove);
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
