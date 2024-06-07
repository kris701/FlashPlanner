using FlashPlanner.Heuristics;
using FlashPlanner.Models;
using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search
{
    /// <summary>
    /// Greedy Search with <seealso href="https://ojs.aaai.org/index.php/ICAPS/article/view/13678">Under-Approximation Refinement</seealso>
    /// </summary>
    public class GreedyBFSUAR : BaseHeuristicPlanner
    {
        /// <summary>
        /// Logging event for the front end
        /// </summary>
        public override event LogEventHandler? DoLog;

        private int _operatorsUsed = -1;
        private readonly OperatorRPG _graphGenerator;
        private HashSet<StateMove> _fullyClosed = new HashSet<StateMove>();
        private readonly Dictionary<int, List<Operator>> _relaxedCache = new Dictionary<int, List<Operator>>();

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        public GreedyBFSUAR(IHeuristic heuristic) : base(heuristic)
        {
            _graphGenerator = new OperatorRPG();
        }

        internal override ActionPlan? Solve(SASStateSpace state)
        {
            // Initial Operator Subset
            _operatorsUsed = -1;
            var operators = GetInitialOperators();
            _openList = InitializeQueue(state);
            _fullyClosed = new HashSet<StateMove>();
            bool haveOnce = false;

            while (!Abort)
            {
                // Refinement Guards
                if (_openList.Count == 0 || _openList.Peek().hValue == int.MaxValue)
                    operators = RefineOperators(operators);

                var stateMove = ExpandBestState();
                int best = stateMove.hValue;
                int current = int.MaxValue;
                foreach (var op in operators)
                {
                    if (Abort) break;
                    if (stateMove.State.IsApplicable(op))
                    {
                        var newMove = GenerateNewState(stateMove, op);
                        if (!IsVisited(newMove) && !_fullyClosed.Contains(newMove))
                        {
                            var value = Heuristic.GetValue(stateMove, newMove.State, operators.ToList());
                            if (value < current)
                                current = value;
                            newMove.hValue = value;
                            QueueOpenList(stateMove, newMove, op);
                            if (newMove.State.IsInGoal())
                                return GeneratePlanChain(newMove);
                        }
                    }
                }

                if (current > best)
                {
                    if (!haveOnce)
                    {
                        operators = RefineOperators(operators, _closedList);
                        haveOnce = true;
                    }
                    else
                        operators = RefineOperators(operators, new HashSet<StateMove>() { stateMove });
                }
            }
            return null;
        }

        private List<Operator> GetInitialOperators()
        {
            var operators = _graphGenerator.GenerateReplaxedPlan(
                new RelaxedSASStateSpace(_context),
                _context.SAS.Operators
                );
            if (_graphGenerator.Failed)
                throw new Exception("No relaxed plan could be found from the initial state! Could indicate the problem is unsolvable.");
            return operators;
        }

        private List<Operator> RefineOperators(List<Operator> operators)
        {
            return RefineOperators(operators, _closedList);
        }

        /// <summary>
        /// From the general idea of the paper:
        /// <list type="bullet">
        ///     <item><description>Select the visited states that have the lowest heuristic value. Generate a new subset of operators, based on relaxed plans starting in those states.​</description></item>
        ///     <item><description>If no new operators was found, repeat the previous but with the states with the next lowest heuristic value.​​</description></item>
        ///     <item><description>If there is still no new operators, do it all again, but simply use all applicable actions from the given states, instead of those from relaxed plans.</description></item>
        /// </list>
        /// </summary>
        /// <param name="operators">Set of unrefined operators</param>
        /// <param name="newClosed"></param>
        /// <returns>A set of refined operators</returns>
        private List<Operator> RefineOperators(List<Operator> operators, HashSet<StateMove> newClosed)
        {
            if (newClosed.Count == 0)
                return operators;

            bool refinedOperatorsFound = false;
            bool lookForApplicaple = false;
            int smallestHValue = -1;
            // Refinement Step 2
            while (!refinedOperatorsFound)
            {
                var smallestItem = newClosed.Where(x => x.hValue > smallestHValue).MinBy(x => x.hValue);
                if (smallestItem == null)
                {
                    // Refinement step 3
                    if (_openList.Count != 0)
                        return operators;

                    if (lookForApplicaple)
                    {
                        Abort = true;
                        return operators;
                    }

                    // Refinement Step 4
                    smallestHValue = -1;
                    lookForApplicaple = true;
                }
                else
                {
                    // Refinement Step 1
                    smallestHValue = smallestItem.hValue;
                    var newOperators = new List<Operator>();
                    if (lookForApplicaple)
                        newOperators = GetNewApplicableOperators(smallestHValue, operators, newClosed);
                    else
                        newOperators = GetNewRelaxedOperators(smallestHValue, operators, newClosed);

                    if (newOperators.Count > 0)
                    {
                        ReopenClosedStates(newOperators, newClosed);
                        operators.AddRange(newOperators);
                        refinedOperatorsFound = true;
                    }
                    else if (lookForApplicaple)
                    {
                        var allTotallyClosed = newClosed.Where(x => x.hValue == smallestHValue);
                        foreach (var state in allTotallyClosed)
                        {
                            newClosed.Remove(state);
                            _fullyClosed.Add(state);
                        }
                    }
                }
            }
            if (operators.Count != _operatorsUsed)
                DoLog?.Invoke($"Operators refined! Now has {operators.Count} operators");

            _operatorsUsed = operators.Count;
            return operators;
        }

        private void ReopenClosedStates(List<Operator> newOperators, HashSet<StateMove> closedItems)
        {
            foreach (var closed in closedItems)
            {
                foreach (var newOperator in newOperators)
                {
                    if (closed.State.IsApplicable(newOperator))
                    {
                        _closedList.Remove(closed);
                        _openList.Enqueue(closed, closed.hValue);
                        break;
                    }
                }
            }
        }

        private List<Operator> GetNewRelaxedOperators(int smallestHValue, List<Operator> operators, HashSet<StateMove> newClosed)
        {
            var allSmallest = newClosed.Where(x => x.hValue == smallestHValue).ToList();
            var relaxedPlanOperators = new List<Operator>();
            foreach (var item in allSmallest)
            {
                if (Abort) return new List<Operator>();
                var hash = item.GetHashCode();
                if (_relaxedCache.TryGetValue(hash, out List<Operator>? value))
                    relaxedPlanOperators.AddRange(value.Except(operators).Except(relaxedPlanOperators));
                else
                {
                    var newOps = _graphGenerator.GenerateReplaxedPlan(
                        item.State,
                        _context.SAS.Operators
                        );
                    if (!_graphGenerator.Failed)
                    {
                        _relaxedCache.Add(hash, newOps);
                        relaxedPlanOperators.AddRange(newOps.Except(operators).Except(relaxedPlanOperators));
                    }
                }
            }
            return relaxedPlanOperators;
        }

        private List<Operator> GetNewApplicableOperators(int smallestHValue, List<Operator> operators, HashSet<StateMove> newClosed)
        {
            var allSmallest = newClosed.Where(x => x.hValue == smallestHValue).ToList();
            var applicableOperators = new List<Operator>();
            foreach (var item in allSmallest)
            {
                if (Abort) return new List<Operator>();
                foreach (var op in _context.SAS.Operators)
                {
                    if (!operators.Contains(op))
                    {
                        if (item.State.IsApplicable(op))
                        {
                            applicableOperators.Add(op);
                        }
                    }
                }
            }
            return applicableOperators;
        }
    }
}
