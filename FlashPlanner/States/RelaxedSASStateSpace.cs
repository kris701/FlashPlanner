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
                _state.Add(add.ID);
            Count = _state.Count;
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="op"></param>
        public RelaxedSASStateSpace(SASStateSpace other, Operator op) : base(other)
        {
            foreach (var add in op.Add)
                _state.Add(add.ID);
            Count = _state.Count;
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
                    _state.Add(add.ID);
            Count = _state.Count;
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
                    _state.Add(add.ID);
            Count = _state.Count;
        }
    }
}
