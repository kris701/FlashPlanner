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
        public List<Operator> Operators { get; set; }
        /// <summary>
        /// What propositions (facts) are true
        /// </summary>
        public HashSet<int> Propositions { get; set; }

        /// <summary>
        /// Main constuctor
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="propositions"></param>
        public Layer(List<Operator> actions, HashSet<int> propositions)
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
