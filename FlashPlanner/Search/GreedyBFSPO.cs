using FlashPlanner.Heuristics;
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
    public class GreedyBFSPO : BaseHeuristicPlanner
    {
        private readonly OperatorRPG _graphGenerator;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        public GreedyBFSPO(IHeuristic heuristic) : base(heuristic)
        {
            _graphGenerator = new OperatorRPG();
        }

        internal override ActionPlan? Solve(SASStateSpace state)
        {
            var preferedOperators = GetPreferredOperators();
            var preferredQueue = InitializeQueue(Heuristic, state, preferedOperators);

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
                            var newMove = GenerateNewState(stateMove, op);
                            if (newMove.State.IsInGoal())
                                return GeneratePlanChain(newMove);
                            if (!_closedList.Contains(newMove) && !preferredQueue.Contains(newMove))
                            {
                                var value = Heuristic.GetValue(stateMove, newMove.State, _declaration.Operators);
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
                new SASStateSpace(_declaration),
                _declaration.Operators
                );
            if (_graphGenerator.Failed)
                throw new Exception("No relaxed plan could be found from the initial state! Could indicate the problem is unsolvable.");
            return operators.ToList();
        }
    }
}
