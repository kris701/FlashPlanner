﻿using FlashPlanner.Heuristics;
using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search.Classical
{
    /// <summary>
    /// Greedy Best First Search with Preferred Operators
    /// (<seealso href="https://ai.dmi.unibas.ch/papers/helmert-jair06.pdf">Helmert 2006</seealso>).
    /// The preferred operators are extracted from a relaxed plan of the problem
    /// </summary>
    public class GreedyBFSPO : BaseClassicalSearch
    {
        private readonly OperatorRPG _graphGenerator;
        public GreedyBFSPO(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
            _graphGenerator = new OperatorRPG(decl);
        }

        internal override ActionPlan? Solve(IHeuristic h, SASStateSpace state)
        {
            var preferedOperators = GetPreferredOperators();
            var preferredQueue = InitializeQueue(h, state, preferedOperators);

            int iteration = 0;
            while (!Abort && (_openList.Count > 0 || preferredQueue.Count > 0))
            {
                if (iteration++ % 2 == 0)
                {
                    if (preferredQueue.Count == 0)
                        continue;

                    var stateMove = ExpandBestState(preferredQueue);

                    foreach (var op in preferedOperators)
                    {
                        if (Abort) break;
                        if (stateMove.State.IsApplicable(op))
                        {
                            var newMove = new StateMove(GenerateNewState(stateMove.State, op));
                            if (newMove.State.IsInGoal())
                                return new ActionPlan(GeneratePlanChain(stateMove.Steps, op));
                            if (!_closedList.Contains(newMove) && !preferredQueue.Contains(newMove))
                            {
                                var value = h.GetValue(stateMove, newMove.State, Declaration.Operators);
                                newMove.Steps = new List<Operator>(stateMove.Steps) { op };
                                newMove.hValue = value;
                                _openList.Enqueue(newMove, value);
                            }
                        }
                    }
                }
                else
                {
                    if (_openList.Count == 0)
                        continue;

                    var stateMove = ExpandBestState();

                    foreach (var op in Declaration.Operators)
                    {
                        if (Abort) break;
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
                                preferredQueue.Enqueue(newMove, value);
                            }
                        }
                    }
                }
            }
            return null;
        }

        private List<Operator> GetPreferredOperators()
        {
            var operators = _graphGenerator.GenerateReplaxedPlan(
                new SASStateSpace(Declaration),
                Declaration.Operators
                );
            if (_graphGenerator.Failed)
                throw new Exception("No relaxed plan could be found from the initial state! Could indicate the problem is unsolvable.");
            return operators.ToList();
        }
    }
}
