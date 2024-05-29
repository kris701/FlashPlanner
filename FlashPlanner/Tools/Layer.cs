using PDDLSharp.Models.SAS;

namespace FlashPlanner.Tools
{
    /// <summary>
    /// A layer in a relaxed planning graph
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// What operators have been executed
        /// </summary>
        public HashSet<Operator> Operators;
        /// <summary>
        /// What propositions (facts) are true
        /// </summary>
        public HashSet<int> Propositions;

        /// <summary>
        /// Main constuctor
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="propositions"></param>
        public Layer(HashSet<Operator> actions, HashSet<int> propositions)
        {
            Operators = actions;
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
