using FlashPlanner.Search.Heuristics;
using FlashPlanner.Search.HeuristicsCollections;
using FlashPlanner.Search.Tools;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;

namespace FlashPlanner.Search.Aliases
{
    public class SatSimple : IHeuristic
    {
        public int Evaluations => _inner.Evaluations;
        private readonly IHeuristic _inner;

        public SatSimple(SASDecl decl)
        {
            _inner = new hColSum(new List<IHeuristic>() {
                new hWeighted(new hGoal(), 10000),
                new hFF(decl)
            });
        }

        public int GetValue(StateMove parent, ISASState state, List<Operator> operators) => _inner.GetValue(parent, state, operators);
        public void Reset() => _inner.Reset();
    }
}
