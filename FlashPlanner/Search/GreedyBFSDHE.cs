using FlashPlanner.Heuristics;
using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search.Classical
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

        internal override ActionPlan? Solve(SASStateSpace state)
        {
            _isEvaluated.Clear();

            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                if (_isEvaluated.ContainsKey(stateMove) && !_isEvaluated[stateMove])
                    stateMove.hValue = Heuristic.GetValue(stateMove, stateMove.State, _declaration.Operators);

                bool lowerFound = false;
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
                            if (lowerFound)
                            {
                                newMove.hValue = stateMove.hValue;
                                _isEvaluated.Add(newMove, false);
                                _openList.Enqueue(newMove, stateMove.hValue);
                            }
                            else
                            {
                                var value = Heuristic.GetValue(stateMove, newMove.State, _declaration.Operators);
                                if (value < stateMove.hValue)
                                    lowerFound = true;
                                newMove.hValue = value;
                                _isEvaluated.Add(newMove, true);
                                _openList.Enqueue(newMove, value);
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
