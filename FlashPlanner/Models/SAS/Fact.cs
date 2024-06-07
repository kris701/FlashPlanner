using PDDLSharp.Tools;

namespace FlashPlanner.Models.SAS
{
    /// <summary>
    /// Representation of a Fact in the state space
    /// </summary>
    public class Fact
    {
        /// <summary>
        /// ID of this fact
        /// </summary>
        public int ID = -1;
        /// <summary>
        /// Name of this fact 
        /// </summary>
        public string Name;
        /// <summary>
        /// Arguments for this fact
        /// </summary>
        public string[] Arguments;

        private int _hashCache = -1;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        public Fact(string name, params string[] arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        /// <summary>
        /// Constructor with ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="arguments"></param>
        public Fact(int id, string name, params string[] arguments) : this(name, arguments)
        {
            ID = id;
        }

        /// <summary>
        /// Hashcode implementation.The order is important!
        /// Based on: https://stackoverflow.com/a/30758270
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (_hashCache != -1)
                return _hashCache;
            const int seed = 487;
            const int modifier = 31;
            unchecked
            {
                _hashCache = 50 * Name.GetHashCode() + Arguments.Length * Arguments.Aggregate(seed, (current, item) =>
                    (current * modifier) * item.GetHashCode());
                return _hashCache;
            }
        }

        /// <summary>
        /// Equals is just based on the ID of the fact, since the translator only outputs unique IDs
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is Fact f)
            {
                if (ID != f.ID)
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
            if (obj is Fact f)
            {
                if (Name != f.Name) return false;
                if (!EqualityHelper.AreListsEqual(Arguments, f.Arguments)) return false;
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
        /// Helper method to copy a Fact
        /// </summary>
        /// <returns></returns>
        public Fact Copy()
        {
            var arguments = new string[Arguments.Length];
            for (int i = 0; i < Arguments.Length; i++)
                arguments[i] = Arguments[i];
            var newFact = new Fact(Name, arguments);
            newFact.ID = ID;
            return newFact;
        }
    }
}
