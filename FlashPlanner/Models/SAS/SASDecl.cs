using PDDLSharp.Tools;

namespace FlashPlanner.Models.SAS
{
    /// <summary>
    /// Internal representation of the SAS task
    /// </summary>
    public class SASDecl
    {
        /// <summary>
        /// The same operators as in the Operators list, but with bitmasks
        /// </summary>
        public List<Operator> Operators;

        /// <summary>
        /// Goal facts
        /// </summary>
        public Fact[] Goal;
        /// <summary>
        /// Goal state as a bitmask
        /// </summary>
        public BitMask GoalMask;

        /// <summary>
        /// Init facts
        /// </summary>
        public Fact[] Init;
        /// <summary>
        /// Init state as a bitmask
        /// </summary>
        public BitMask InitMask;

        /// <summary>
        /// Facts in the state space (used for the bitmasks)
        /// </summary>
        public int Facts;

        private int _hashCache = -1;
        private readonly Dictionary<int, Operator> _operatorDict;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="operators"></param>
        /// <param name="goal"></param>
        /// <param name="init"></param>
        /// <param name="factCount"></param>
        public SASDecl(List<Operator> operators, Fact[] goal, Fact[] init, int factCount)
        {
            Operators = operators;
            Goal = goal;
            Init = init;
            _operatorDict = new Dictionary<int, Operator>();
            foreach (var op in operators)
            {
                if (_operatorDict.ContainsKey(op.ID))
                    continue;
                _operatorDict.Add(op.ID, op);
            }

            GoalMask = new BitMask(factCount);
            foreach (var fact in goal)
                GoalMask[fact.ID] = true;
            Init = init;
            InitMask = new BitMask(factCount);
            foreach (var fact in init)
                InitMask[fact.ID] = true;
            Facts = factCount;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public SASDecl() : this(new List<Operator>(), new Fact[0], new Fact[0], 0)
        {
        }

        /// <summary>
        /// Copy a given SASDecl
        /// </summary>
        /// <returns></returns>
        public SASDecl Copy()
        {
            var operators = new List<Operator>();
            var goal = new List<Fact>();
            var init = new List<Fact>();

            foreach (var op in Operators)
                operators.Add(op.Copy());
            foreach (var g in Goal)
                goal.Add(g.Copy());
            foreach (var i in Init)
                init.Add(i.Copy());

            return new SASDecl(operators, goal.ToArray(), init.ToArray(), Facts);
        }

        /// <summary>
        /// Equals implementation
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is SASDecl other)
            {
                if (!EqualityHelper.AreListsEqual(Operators, other.Operators)) return false;
                if (!EqualityHelper.AreListsEqual(Goal, other.Goal)) return false;
                if (!EqualityHelper.AreListsEqual(Init, other.Init)) return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hashcode implementation.
        /// Only calculated once pr instantiation
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (_hashCache != -1)
                return _hashCache;
            _hashCache = 1;
            foreach (var child in Operators)
                _hashCache ^= child.GetHashCode();
            foreach (var child in Goal)
                _hashCache ^= child.GetHashCode();
            foreach (var child in Init)
                _hashCache ^= child.GetHashCode();
            return _hashCache;
        }

        /// <summary>
        /// Helper method to get an operator by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Operator GetOperatorByID(int id) => _operatorDict[id];
    }
}
