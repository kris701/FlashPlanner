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
        /// <param name="factHashes"></param>
        public RelaxedSASStateSpace(SASDecl declaration, Dictionary<int, int> factHashes) : base(declaration, factHashes)
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
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public RelaxedSASStateSpace(RelaxedSASStateSpace other) : base(other)
        {
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="op"></param>
        public RelaxedSASStateSpace(RelaxedSASStateSpace other, Operator op) : base(other)
        {
            foreach (var add in op.Add)
                _state[add.ID] = true;
            SetCount();
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="op"></param>
        public RelaxedSASStateSpace(SASStateSpace other, Operator op) : base(other)
        {
            foreach (var add in op.Add)
                _state[add.ID] = true;
            SetCount();
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="ops"></param>
        public RelaxedSASStateSpace(SASStateSpace other, List<Operator> ops) : this(other)
        {
            foreach (var op in ops)
                foreach (var add in op.Add)
                    _state[add.ID] = true;
            SetCount();
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="ops"></param>
        public RelaxedSASStateSpace(RelaxedSASStateSpace other, List<Operator> ops) : this(other)
        {
            foreach (var op in ops)
                foreach (var add in op.Add)
                    _state[add.ID] = true;
            SetCount();
        }
    }
}
