using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Heuristics
{
    public class hMax : BaseHeuristic
    {
        private FactRPG _graphGenerator;
        public hMax()
        {
            _graphGenerator = new FactRPG();
        }

        public override int GetValue(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            Evaluations++;
            var max = 0;
            var dict = _graphGenerator.GenerateRelaxedGraph(state, operators);
            foreach (var fact in state.Declaration.Goal)
            {
                if (!dict.ContainsKey(fact.ID))
                    return int.MaxValue;
                var factCost = dict[fact.ID];
                if (factCost == int.MaxValue)
                    return int.MaxValue;
                if (factCost > max)
                    max = factCost;
            }
            return max;
        }

        public override void Reset()
        {
            base.Reset();
            _graphGenerator = new FactRPG();
        }
    }
}
