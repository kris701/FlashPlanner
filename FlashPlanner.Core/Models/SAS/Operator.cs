using PDDLSharp.Tools;

namespace FlashPlanner.Models.SAS
{
    /// <summary>
    /// An operator in the SAS declaration
    /// </summary>
    public class Operator
    {
        /// <summary>
        /// ID of this operator
        /// </summary>
        public int ID = 0;
        /// <summary>
        /// Name of this operator
        /// </summary>
        public string Name;
        /// <summary>
        /// Arguments of this operator
        /// </summary>
        public string[] Arguments;

        /// <summary>
        /// Preconditions of this operator
        /// </summary>
        public Fact[] Pre;
        /// <summary>
        /// Bitmask for precondiotions
        /// </summary>
        public BitMask PreMask;
        /// <summary>
        /// Cached size of the preconditions
        /// </summary>
        public int PreCount;

        /// <summary>
        /// The add list of this operator
        /// </summary>
        public Fact[] Add;
        /// <summary>
        /// Bitmask for add list
        /// </summary>
        public BitMask AddMask;
        /// <summary>
        /// Caches size of the add list
        /// </summary>
        public int AddCount;

        /// <summary>
        /// The delete list of this operator
        /// </summary>
        public Fact[] Del;
        /// <summary>
        /// Bitmask for delete list
        /// </summary>
        public BitMask DelMask;
        /// <summary>
        /// Cached size of the delete list
        /// </summary>
        public int DelCount;

        private int _hashCache = -1;
        private readonly int _factCount;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        /// <param name="pre"></param>
        /// <param name="add"></param>
        /// <param name="del"></param>
        /// <param name="factCount"></param>
        public Operator(string name, string[] arguments, Fact[] pre, Fact[] add, Fact[] del, int factCount)
        {
            Name = name;
            Arguments = arguments;
            Pre = pre;
            PreCount = pre.Length;
            Add = add;
            AddCount = add.Length;
            Del = del;
            DelCount = del.Length;
            PreMask = new BitMask(factCount);
            foreach (var fact in pre)
                PreMask[fact.ID] = true;
            PreCount = pre.Length;
            AddMask = new BitMask(factCount);
            foreach (var fact in add)
                AddMask[fact.ID] = true;
            DelMask = new BitMask(factCount);
            foreach (var fact in del)
                DelMask[fact.ID] = true;
            _factCount = factCount;
        }

        /// <summary>
        /// Hashcode implementation.
        /// Only calculated once pr instantiation.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (_hashCache != -1)
                return _hashCache;

            _hashCache = Name.GetHashCode();
            foreach (var arg in Arguments)
                _hashCache ^= arg.GetHashCode();
            foreach (var pre in Pre)
                _hashCache ^= pre.GetHashCode();
            foreach (var del in Del)
                _hashCache ^= del.GetHashCode();
            foreach (var add in Add)
                _hashCache ^= add.GetHashCode();

            return _hashCache;
        }

        /// <summary>
        /// Equals is just based on the ID of the operator, since the translator only outputs unique IDs
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is Operator o)
            {
                if (ID != o.ID)
                    return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Backup equals method, to compare the actual content instead of just the IDs
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ContentEquals(object? obj)
        {
            if (obj is Operator o)
            {
                if (Name != o.Name) return false;
                if (!EqualityHelper.AreListsEqual(Arguments, o.Arguments)) return false;
                if (PreCount != o.PreCount) return false;
                if (!Pre.All(x => o.Pre.Contains(x))) return false;
                if (AddCount != o.AddCount) return false;
                if (!Add.All(x => o.Add.Contains(x))) return false;
                if (DelCount != o.DelCount) return false;
                if (!Del.All(x => o.Del.Contains(x))) return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Visually pleasing tostring implenmentation
        /// </summary>
        /// <returns></returns>
        public override string? ToString()
        {
            var retStr = Name;
            foreach (var arg in Arguments)
                retStr += $" {arg}";
            return retStr;
        }

        /// <summary>
        /// Helper method to copy a operator
        /// </summary>
        /// <returns></returns>
        public Operator Copy()
        {
            var arguments = new string[Arguments.Length];
            var pre = new Fact[PreCount];
            var add = new Fact[AddCount];
            var del = new Fact[DelCount];

            for (int i = 0; i < Arguments.Length; i++)
                arguments[i] = Arguments[i];
            for (int i = 0; i < PreCount; i++)
                pre[i] = Pre[i].Copy();
            for (int i = 0; i < AddCount; i++)
                add[i] = Add[i].Copy();
            for (int i = 0; i < DelCount; i++)
                del[i] = Del[i].Copy();

            var newOp = new Operator(Name, arguments, pre, add, del, _factCount);
            newOp.ID = ID;
            return newOp;
        }
    }
}
