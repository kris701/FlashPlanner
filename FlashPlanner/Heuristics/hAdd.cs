using FlashPlanner.Models;
using FlashPlanner.RelaexPlanningGraphs;
using FlashPlanner.States;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Heuristics
{
    /// <summary>
    /// Retuns the sum of actions needed to achive every goal fact
    /// </summary>
    public class hAdd : BaseHeuristic
    {
        private readonly FactRPG _graphGenerator;
        /// <summary>
        /// Main constructor
        /// </summary>
        public hAdd()
        {
            _graphGenerator = new FactRPG();
        }

        internal override int GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            var cost = 0;
            var dict = _graphGenerator.GenerateRelaxedGraph(state, operators);
            foreach (var fact in state.Context.SAS.Goal)
            {
                if (!dict.ContainsKey(fact.ID))
                    return int.MaxValue;
                var factCost = dict[fact.ID];
                if (factCost == int.MaxValue)
                    return int.MaxValue;
                cost += factCost;
            }
            return cost;
        }
    }
}
