using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.RelaxedPlanningGraphs;
using FlashPlanner.Core.States;
using PDDLSharp.Models.FastDownward.Plans;

namespace FlashPlanner.Core.Search
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

        internal override ActionPlan? Solve()
        {
            var preferedOperators = GetPreferredOperators();
            var preferredQueue = InitializeQueue(new SASStateSpace(_context));

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
                            if (!IsVisited(newMove) && !preferredQueue.Contains(newMove))
                            {
                                var value = Heuristic.GetValue(stateMove, newMove.State, _context.SAS.Operators);
                                newMove.hValue = value;
                                QueueOpenList(stateMove, newMove, op);
                                if (newMove.State.IsInGoal())
                                    return GeneratePlanChain(newMove);
                            }
                        }
                    }
                }
                else
                {
                    if (_openList.Count == 0)
                        continue;

                    var stateMove = ExpandBestState();

                    foreach (var opID in _context.ApplicabilityGraph[stateMove.Operator])
                    {
                        var op = _context.SAS.GetOperatorByID(opID);
                        if (Abort) break;
                        if (stateMove.State.IsApplicable(op))
                        {
                            var newMove = GenerateNewState(stateMove, op);
                            if (!IsVisited(newMove) && !preferredQueue.Contains(newMove))
                            {
                                var value = Heuristic.GetValue(stateMove, newMove.State, _context.SAS.Operators);
                                newMove.hValue = value;
                                QueueOpenList(stateMove, newMove, op);
                                if (newMove.State.IsInGoal())
                                    return GeneratePlanChain(newMove);
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
                new SASStateSpace(_context),
                _context.SAS.Operators
                );
            if (_graphGenerator.Failed)
                throw new Exception("No relaxed plan could be found from the initial state! Could indicate the problem is unsolvable.");
            return operators.ToList();
        }
    }
}
