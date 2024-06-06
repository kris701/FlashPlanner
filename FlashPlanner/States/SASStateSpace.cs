using PDDLSharp.Models.SAS;
using PDDLSharp.Tools;
using System.Collections;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FlashPlanner.Tests")]
namespace FlashPlanner.States
{
    /// <summary>
    /// Representation of a state space
    /// </summary>
    public class SASStateSpace : IEnumerable<int>
    {
        /// <summary>
        /// A reference <seealso cref="SASDecl"/> that this state space if for
        /// </summary>
        public SASDecl Declaration { get; internal set; }
        /// <summary>
        /// Amount of facts in the state space.
        /// </summary>
        public int Count;

        internal HashSet<int> _state;
        private int _hashCache = -1;

        /// <summary>
        /// Main initializer constructor
        /// </summary>
        /// <param name="declaration"></param>
        public SASStateSpace(SASDecl declaration)
        {
            Declaration = declaration;
            _state = new HashSet<int>();
            foreach (var fact in declaration.Init)
                _state.Add(fact.ID);
            Count = _state.Count;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public SASStateSpace(SASStateSpace other)
        {
            Declaration = other.Declaration;
            var newState = new int[other._state.Count];
            Buffer.BlockCopy(other._state.ToArray(), 0, newState, 0, other._state.Count * sizeof(int));
            //other._state.CopyTo(newState);
            _state = newState.ToHashSet();
            Count = _state.Count;
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="op"></param>
        public SASStateSpace(SASStateSpace other, Operator op) : this(other)
        {
            foreach(var del in op.Del)
                _state.Remove(del.ID);
            foreach (var add in op.Add)
                _state.Add(add.ID);
            Count = _state.Count;
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="ops"></param>
        public SASStateSpace(SASStateSpace other, List<Operator> ops) : this(other)
        {
            foreach (var op in ops)
            {
                foreach (var del in op.Del)
                    _state.Remove(del.ID);
                foreach (var add in op.Add)
                    _state.Add(add.ID);
                Count = _state.Count;
            }
        }

        /// <summary>
        /// Get all the facts in the state space.
        /// </summary>
        /// <returns></returns>
        public HashSet<int> GetFacts() => new HashSet<int>(_state);

        /// <summary>
        /// If the state contains a given fact, by its SAS ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(int id) => _state.Contains(id);

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
                foreach (var item in other._state)
                    if (!_state.Contains(item))
                        return false;
                foreach (var item in _state)
                    if (!other._state.Contains(item))
                        return false;
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
            int hash = Count;
            foreach (var item in _state)
                hash ^= item;
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
            foreach(var pre in op.Pre)
                if (!_state.Contains(pre.ID))
                    return false;
            return true;
        }

        /// <summary>
        /// Checks if we are in the goal.
        /// </summary>
        /// <returns></returns>
        public bool IsInGoal()
        {
            if (Count < Declaration.Goal.Count) return false;
            foreach (var fact in Declaration.Goal)
                if (!Contains(fact.ID))
                    return false;
            return true;
        }

        /// <summary>
        /// Iterator to iterate through the facts in the state space.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator() => _state.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _state.GetEnumerator();
    }
}
