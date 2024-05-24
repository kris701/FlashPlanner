using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Heuristics
{
    public class hFF : BaseHeuristic
    {
        private OperatorRPG _graphGenerator;

        public hFF()
        {
            _graphGenerator = new OperatorRPG();
        }

        public override int GetValue(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            Evaluations++;
            var relaxedPlan = _graphGenerator.GenerateReplaxedPlan(
                state,
                operators);
            if (_graphGenerator.Failed)
                return int.MaxValue;
            return relaxedPlan.Count;
        }

        public override void Reset()
        {
            base.Reset();
            _graphGenerator = new OperatorRPG();
        }
    }
}
