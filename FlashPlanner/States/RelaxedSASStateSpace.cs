using PDDLSharp.Models.SAS;

namespace FlashPlanner.States
{
    public class RelaxedSASStateSpace : SASStateSpace
    {
        public RelaxedSASStateSpace(SASDecl declaration) : base(declaration)
        {
        }

        public RelaxedSASStateSpace(SASDecl declaration, HashSet<Fact> state) : base(declaration, state)
        {
        }

        public override void ExecuteNode(Operator node)
        {
            foreach (var fact in node.Add)
                State.Add(fact);
        }

        public override RelaxedSASStateSpace Copy()
        {
            var newState = new Fact[State.Count];
            State.CopyTo(newState);
            return new RelaxedSASStateSpace(Declaration, newState.ToHashSet());
        }
    }
}
