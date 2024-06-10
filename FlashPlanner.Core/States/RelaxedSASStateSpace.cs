using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;

namespace FlashPlanner.Core.States
{
    /// <summary>
    /// Representation of a relaxed state space
    /// </summary>
    public class RelaxedSASStateSpace : SASStateSpace
    {
        /// <summary>
        /// Main initializer constructor
        /// </summary>
        /// <param name="context"></param>
        public RelaxedSASStateSpace(TranslatorContext context) : base(context)
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
            _hashCache = other._hashCache;
            foreach (var add in op.Add)
            {
                if (!_state[add.ID])
                    _hashCache ^= Context.FactHashes[add.ID];
                _state[add.ID] = true;
            }
            Count = _state.GetTrueBits();
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="op"></param>
        public RelaxedSASStateSpace(SASStateSpace other, Operator op) : base(other)
        {
            _hashCache = other._hashCache;
            foreach (var add in op.Add)
            {
                if (!_state[add.ID])
                    _hashCache ^= Context.FactHashes[add.ID];
                _state[add.ID] = true;
            }
            Count = _state.GetTrueBits();
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="ops"></param>
        public RelaxedSASStateSpace(SASStateSpace other, List<Operator> ops) : this(other)
        {
            _hashCache = other._hashCache;
            foreach (var op in ops)
            {
                foreach (var add in op.Add)
                {
                    if (!_state[add.ID])
                        _hashCache ^= Context.FactHashes[add.ID];
                    _state[add.ID] = true;
                }
            }
            Count = _state.GetTrueBits();
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="ops"></param>
        public RelaxedSASStateSpace(RelaxedSASStateSpace other, List<Operator> ops) : this(other)
        {
            _hashCache = other._hashCache;
            foreach (var op in ops)
            {
                foreach (var add in op.Add)
                {
                    if (!_state[add.ID])
                        _hashCache ^= Context.FactHashes[add.ID];
                    _state[add.ID] = true;
                }
            }
            Count = _state.GetTrueBits();
        }
    }
}
