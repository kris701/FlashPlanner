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
    public class GreedyBFSDHE : BaseClassicalSearch
    {
        public GreedyBFSDHE(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
        }

        private readonly Dictionary<StateMove, bool> _isEvaluated = new Dictionary<StateMove, bool>();

        internal override ActionPlan? Solve(IHeuristic h, SASStateSpace state)
        {
            _isEvaluated.Clear();

            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                if (_isEvaluated.ContainsKey(stateMove) && !_isEvaluated[stateMove])
                    stateMove.hValue = h.GetValue(stateMove, stateMove.State, Declaration.Operators);

                bool lowerFound = false;
                foreach (var op in Declaration.Operators)
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
                                var value = h.GetValue(stateMove, newMove.State, Declaration.Operators);
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
