using PDDLSharp.Models.SAS;
using System.Collections;

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
        public SASDecl Declaration { get; }
        /// <summary>
        /// Amount of facts in the state space.
        /// </summary>
        public int Count => _state.Count;

        private readonly HashSet<int> _state;
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
                Add(fact);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public SASStateSpace(SASStateSpace other)
        {
            Declaration = other.Declaration;
            var newState = new int[other._state.Count];
            other._state.CopyTo(newState);
            _state = newState.ToHashSet();
        }

        /// <summary>
        /// Get all the facts in the state space.
        /// </summary>
        /// <returns></returns>
        public HashSet<int> GetFacts() => new HashSet<int>(_state);

        /// <summary>
        /// Add a fact
        /// </summary>
        /// <param name="pred"></param>
        /// <returns></returns>
        public bool Add(Fact pred)
        {
            var changed = _state.Add(pred.ID);
            if (changed)
                _hashCache = -1;
            return changed;
        }
        /// <summary>
        /// Remove a fact
        /// </summary>
        /// <param name="pred"></param>
        /// <returns></returns>
        public bool Del(Fact pred)
        {
            var changed = _state.Remove(pred.ID);
            if (changed)
                _hashCache = -1;
            return changed;
        }
        /// <summary>
        /// If the state contains a given fact
        /// </summary>
        /// <param name="pred"></param>
        /// <returns></returns>
        public bool Contains(Fact pred) => Contains(pred.ID);
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
                if (GetHashCode() != other.GetHashCode()) return false;
                if (other._state.Count != _state.Count) return false;
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
            int hash = _state.Count;
            foreach (var item in _state)
                hash ^= item;
            _hashCache = hash;
            return hash;
        }

        /// <summary>
        /// Execute a given operator on this state space.
        /// It first deletes, then adds.
        /// </summary>
        /// <param name="node"></param>
        public virtual void Execute(Operator node)
        {
            foreach (var fact in node.Del)
                Del(fact);
            foreach (var fact in node.Add)
                Add(fact);
        }

        /// <summary>
        /// Checks if an operators preconditions are all valid
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsApplicable(Operator node)
        {
            foreach (var fact in node.Pre)
                if (!Contains(fact))
                    return false;
            return true;
        }

        /// <summary>
        /// Checks if we are in the goal.
        /// </summary>
        /// <returns></returns>
        public bool IsInGoal()
        {
            foreach (var fact in Declaration.Goal)
                if (!Contains(fact))
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
