using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.RelaxedPlanningGraphs;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.Heuristics
{
    /// <summary>
    /// Main hFF implementation based on relaxed planning graphs
    /// </summary>
    public class hFF : BaseHeuristic
    {
        private readonly OperatorRPG _graphGenerator;

        /// <summary>
        /// Main constructor
        /// </summary>
        public hFF()
        {
            _graphGenerator = new OperatorRPG();
        }

        internal override uint GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            var relaxedPlan = _graphGenerator.GenerateReplaxedPlan(
                state,
                operators);
            if (_graphGenerator.Failed)
                return uint.MaxValue;
            return (uint)relaxedPlan.Count;
        }
    }
}
