using FlashPlanner.Translators;
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
        /// A dictionary of hash values for facts
        /// </summary>
        public Dictionary<int, int> FactHashes = new Dictionary<int, int>();
        /// <summary>
        /// Amount of facts in the state space.
        /// </summary>
        public int Count;

        internal BitArray _state;
        internal int _hashCache = -1;

        /// <summary>
        /// Main initializer constructor
        /// </summary>
        /// <param name="declaration"></param>
        /// <param name="factHashes"></param>
        public SASStateSpace(SASDecl declaration, Dictionary<int, int> factHashes)
        {
            Declaration = declaration;
            _state = new BitArray(declaration.Facts);
            var count = 0;
            foreach (var fact in declaration.Init)
            {
                _state.Set(fact.ID, true);
                count++;
            }
            Count = count;
            FactHashes = factHashes;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public SASStateSpace(SASStateSpace other)
        {
            Declaration = other.Declaration;
            _state = new BitArray(other._state);
            SetCount();
            FactHashes = other.FactHashes;
        }

        /// <summary>
        /// Copy and execute constructor
        /// </summary>
        /// <param name="other"></param>
        /// <param name="op"></param>
        public SASStateSpace(SASStateSpace other, Operator op) : this(other)
        {
            foreach (var del in op.Del)
                _state[del.ID] = false;
            foreach (var add in op.Add)
                _state[add.ID] = true;
            SetCount();
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
                    _state[del.ID] = false;
                foreach (var add in op.Add)
                    _state[add.ID] = true;
            }
            SetCount();
        }

        //https://stackoverflow.com/a/14354311
        internal void SetCount()
        {
            var ints = new int[(_state.Count >> 5) + 1];
            _state.CopyTo(ints, 0);
            var count = 0;

            // fix for not truncated bits in last integer that may have been set to true with SetAll()
            ints[ints.Length - 1] &= ~(-1 << (_state.Count % 32));

            for (int i = 0; i < ints.Length; i++)
            {
                int c = ints[i];
                // magic (http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel)
                unchecked
                {
                    c = c - ((c >> 1) & 0x55555555);
                    c = (c & 0x33333333) + ((c >> 2) & 0x33333333);
                    c = ((c + (c >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
                }
                count += c;
            }
            Count = count;
        }

        /// <summary>
        /// Get all the facts in the state space.
        /// </summary>
        /// <returns></returns>
        public HashSet<int> GetFacts()
        {
            var set = new HashSet<int>();
            for (int i = 0; i < _state.Length; i++)
                if (_state[i])
                    set.Add(i);

            return set;
        }

        /// <summary>
        /// If the state contains a given fact, by its SAS ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains(int id) => _state[id];

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
                for(int i = 0; i < Count; i++)
                    if (other._state[i] != _state[i]) 
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
            int hash = 359412394;
            for (int i = 0; i < _state.Count; i++)
                if (_state[i])
                    hash ^= FactHashes[i];
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
                if (!_state[pre.ID])
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
                if (!_state[fact.ID])
                    return false;
            return true;
        }

        /// <summary>
        /// Iterator to iterate through the facts in the state space.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator() => GetFacts().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetFacts().GetEnumerator();
    }
}
