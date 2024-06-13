using FlashPlanner.Core.Models.SAS;

namespace FlashPlanner.Core.RelaxedPlanningGraphs
{
    /// <summary>
    /// A layer in a relaxed planning graph
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// What operators have been executed
        /// </summary>
        public Dictionary<uint, Operator> Operators;
        /// <summary>
        /// What propositions (facts) are true
        /// </summary>
        public HashSet<uint> Propositions;

        /// <summary>
        /// Main constuctor
        /// </summary>
        /// <param name="operators"></param>
        /// <param name="propositions"></param>
        public Layer(List<Operator> operators, HashSet<uint> propositions)
        {
            Operators = new Dictionary<uint, Operator>(operators.Count);
            foreach (var op in operators)
                Operators.Add(op.ID, op);
            Propositions = propositions;
        }

        /// <summary>
        /// Nicer tostring implementation
        /// </summary>
        /// <returns></returns>
        public override string? ToString()
        {
            return $"Ops: {Operators.Count}, Props: {Propositions.Count}";
        }
    }
}
