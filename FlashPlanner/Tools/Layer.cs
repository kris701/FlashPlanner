using PDDLSharp.Models.SAS;

namespace FlashPlanner.Tools
{
    public class Layer
    {
        public List<Operator> Operators { get; set; }
        public HashSet<int> Propositions { get; set; }

        public Layer(List<Operator> actions, HashSet<int> propositions)
        {
            Operators = actions;
            Propositions = propositions;
        }

        public Layer()
        {
            Operators = new List<Operator>();
            Propositions = new HashSet<int>();
        }

        public override string? ToString()
        {
            return $"Ops: {Operators.Count}, Props: {Propositions.Count}";
        }
    }
}
