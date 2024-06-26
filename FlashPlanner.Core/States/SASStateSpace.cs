﻿using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using System.Collections;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FlashPlanner.Tests")]
namespace FlashPlanner.Core.States
{
    /// <summary>
    /// Representation of a state space
    /// </summary>
    public class SASStateSpace : IEnumerable<uint>
    {
        /// <summary>
        /// A reference to the translated context
        /// </summary>
        public TranslatorContext Context;
        /// <summary>
        /// Amount of facts in the state space.
        /// </summary>
        public uint Count;

        internal BitMask _state;
        internal int _hashCache = -1;

        /// <summary>
        /// Main initializer constructor
        /// </summary>
        /// <param name="context"></param>
        public SASStateSpace(TranslatorContext context)
        {
            Context = context;
            _state = new BitMask(Context.SAS.InitMask);
            Count = _state.GetTrueBits();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public SASStateSpace(SASStateSpace other)
        {
            Context = other.Context;
            _state = new BitMask(other._state);
            Count = other.Count;
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="op"></param>
        public SASStateSpace(SASStateSpace other, Operator op) : this(other)
        {
            _hashCache = other._hashCache;
            foreach (var del in op.Del)
                if (_state[del.ID])
                    _hashCache ^= Context.FactHashes[del.ID];
            foreach (var add in op.Add)
                if (!_state[add.ID])
                    _hashCache ^= Context.FactHashes[add.ID];
            _state.NAnd(op.DelMask);
            _state.Or(op.AddMask);
            Count = _state.GetTrueBits();
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="ops"></param>
        public SASStateSpace(SASStateSpace other, List<Operator> ops) : this(other)
        {
            _hashCache = other._hashCache;
            foreach (var op in ops)
            {
                foreach (var del in op.Del)
                    if (_state[del.ID])
                        _hashCache ^= Context.FactHashes[del.ID];
                foreach (var add in op.Add)
                    if (!_state[add.ID])
                        _hashCache ^= Context.FactHashes[add.ID];
                _state.NAnd(op.DelMask);
                _state.Or(op.AddMask);
            }
            Count = _state.GetTrueBits();
        }

        /// <summary>
        /// If the state contains a given fact, by its SAS ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool this[uint id] => _state[id];

        /// <summary>
        /// Equals override for the state
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is SASStateSpace other)
            {
                if (other.Count != Count) return false;
                if (!other._state.Equals(_state)) return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hashcode implementation.
        /// Is only calculated once.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (_hashCache != -1)
                return _hashCache;
            int hash = 359412394;
            for (uint i = 0; i < _state.Length; i++)
                if (_state[i])
                    hash ^= Context.FactHashes[i];
            _hashCache = hash;
            return hash;
        }

        /// <summary>
        /// Checks if an operators preconditions are all valid
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public bool IsApplicable(Operator op)
        {
            if (Count < op.PreCount) return false;
            return op.PreMask.IsSubsetOf(_state);
        }

        /// <summary>
        /// Checks if we are in the goal.
        /// </summary>
        /// <returns></returns>
        public bool IsInGoal() => Context.SAS.GoalMask.IsSubsetOf(_state);

        /// <summary>
        /// Iterator to iterate through the facts in the state space.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<uint> GetEnumerator() => _state.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _state.GetEnumerator();
    }
}
