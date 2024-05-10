using PDDLSharp.Models.SAS;
using System.Collections;

namespace FlashPlanner.States
{
    public class SASStateSpace : IEnumerable<int>
    {
        public SASDecl Declaration { get; }
        public int Count => _state.Count;

        internal HashSet<int> _state;

        public SASStateSpace(SASDecl declaration)
        {
            Declaration = declaration;
            _state = new HashSet<int>();
            foreach (var fact in declaration.Init)
                Add(fact);
        }

        public SASStateSpace(SASStateSpace other)
        {
            Declaration = other.Declaration;
            var newState = new int[other._state.Count];
            other._state.CopyTo(newState);
            _state = newState.ToHashSet();
        }

        public bool Add(Fact pred) => Add(pred.ID);
        public bool Add(int id)
        {
            var changed = _state.Add(id);
            if (changed)
                _hashCache = -1;
            return changed;
        }
        public bool Del(Fact pred) => Del(pred.ID);
        public bool Del(int id)
        {
            var changed = _state.Remove(id);
            if (changed)
                _hashCache = -1;
            return changed;
        }
        public bool Contains(Fact pred) => Contains(pred.ID);
        public bool Contains(int id) => _state.Contains(id);

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

        private int _hashCache = -1;
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

        public virtual void Execute(Operator node)
        {
            foreach (var fact in node.Del)
                Del(fact);
            foreach (var fact in node.Add)
                Add(fact);
        }

        public bool IsApplicable(Operator node)
        {
            foreach (var fact in node.Pre)
                if (!Contains(fact))
                    return false;
            return true;
        }

        public bool IsInGoal()
        {
            foreach (var fact in Declaration.Goal)
                if (!Contains(fact))
                    return false;
            return true;
        }

        public IEnumerator<int> GetEnumerator() => _state.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _state.GetEnumerator();
    }
}
