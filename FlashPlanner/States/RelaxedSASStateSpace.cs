using PDDLSharp.Models.SAS;

namespace FlashPlanner.States
{
    public class RelaxedSASStateSpace : SASStateSpace
    {
        public RelaxedSASStateSpace(SASDecl declaration) : base(declaration)
        {
        }

        public RelaxedSASStateSpace(RelaxedSASStateSpace other) : base(other)
        {
        }

        public RelaxedSASStateSpace(SASStateSpace other) : base(other)
        {
        }

        public override void Execute(Operator node)
        {
            foreach (var fact in node.Add)
                Add(fact);
        }
    }
}
