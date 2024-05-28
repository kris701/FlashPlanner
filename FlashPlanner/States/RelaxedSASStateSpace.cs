using PDDLSharp.Models.SAS;

namespace FlashPlanner.States
{
    /// <summary>
    /// Representation of a relaxed state space
    /// </summary>
    public class RelaxedSASStateSpace : SASStateSpace
    {
        /// <summary>
        /// Main initializer constructor
        /// </summary>
        /// <param name="declaration"></param>
        public RelaxedSASStateSpace(SASDecl declaration) : base(declaration)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public RelaxedSASStateSpace(RelaxedSASStateSpace other) : base(other)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public RelaxedSASStateSpace(SASStateSpace other) : base(other)
        {
        }

        /// <summary>
        /// Execute a given operator on this state space.
        /// Facts are only added here, not deleted.
        /// </summary>
        /// <param name="node"></param>
        public override void Execute(Operator node)
        {
            foreach (var fact in node.Add)
                Add(fact);
        }
    }
}
