using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;
using PDDLSharp.Models.FastDownward.Plans;

namespace FlashPlanner.Core.Search
{
    /// <summary>
    /// Base implementation for heuristic planner engines
    /// </summary>
    public abstract class BaseHeuristicPlanner : LimitedComponent, IPlanner
    {
        /// <summary>
        /// Logging event for the front end
        /// </summary>
        public override event LogEventHandler? DoLog;
        /// <summary>
        /// What heuristic to use
        /// </summary>
        public IHeuristic Heuristic { get; }
        /// <summary>
        /// Amount of generated states
        /// </summary>
        public int Generated { get; private set; }
        /// <summary>
        /// Amount of expanded states
        /// </summary>
        public int Expanded { get; private set; }
        /// <summary>
        /// Amount of heuristic evaluations
        /// </summary>
        public int Evaluations => Heuristic.Evaluations;

        internal TranslatorContext _context;
        internal HashSet<StateMove> _closedList = new HashSet<StateMove>();
        internal RefPriorityQueue<StateMove> _openList = new RefPriorityQueue<StateMove>();
        // This is a map from a given State -> operatorID -> resulting state.
        internal Dictionary<StateMove, StateMove> _planMap = new Dictionary<StateMove, StateMove>();

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        public BaseHeuristicPlanner(IHeuristic heuristic)
        {
            Heuristic = heuristic;
            _context = new TranslatorContext();
        }

        /// <summary>
        /// Get a plan for some <seealso cref="SASDecl"/> on the <seealso cref="IHeuristic"/> provided
        /// </summary>
        /// <param name="decl"></param>
        /// <returns>A plan or null if unsolvable</returns>
        public ActionPlan? Solve(TranslatorContext context)
        {
            Heuristic.Reset();
            _context = context;
            _planMap = new Dictionary<StateMove, StateMove>();

            var state = new SASStateSpace(context);
            if (state.IsInGoal())
                return new ActionPlan(new List<GroundedAction>());

            Start();

            _closedList = new HashSet<StateMove>();
            _openList = InitializeQueue(state);

            Expanded = 0;
            Generated = 0;
            Heuristic.Reset();

            var result = Solve(state);

            Stop();

#if RELEASE
            _closedList.Clear();
            _openList.Clear();
#endif

            return result;
        }

        internal abstract ActionPlan? Solve(SASStateSpace state);

        internal StateMove GenerateNewState(StateMove state, Operator op)
        {
            Generated++;
            var stateMove = new StateMove(state, op);
            return stateMove;
        }

        internal void QueueOpenList(StateMove from, StateMove to, Operator op)
        {
            _planMap.Add(to, from);
            _openList.Enqueue(to, to.hValue);
        }

        internal RefPriorityQueue<StateMove> InitializeQueue(SASStateSpace state)
        {
            var queue = new RefPriorityQueue<StateMove>();
            var fromMove = new StateMove(state);
            queue.Enqueue(fromMove, int.MaxValue);
            return queue;
        }

        internal StateMove ExpandBestState(RefPriorityQueue<StateMove>? from = null)
        {
            if (from == null)
                from = _openList;
            var stateMove = from.Dequeue();
            _closedList.Add(stateMove);
            Expanded++;
            return stateMove;
        }

        internal ActionPlan GeneratePlanChain(StateMove state)
        {
            var chain = new List<GroundedAction>();

            while (_planMap.ContainsKey(state))
            {
                if (state.Operator == -1)
                    break;
                var previousState = _planMap[state];
                chain.Add(GenerateFromOp(_context.SAS.Operators.First(x => x.ID == state.Operator)));
                state = previousState;
            }
            chain.Reverse();

            return new ActionPlan(chain);
        }

        internal GroundedAction GenerateFromOp(Operator op) => new GroundedAction(op.Name, op.Arguments);

        internal bool IsVisited(StateMove state) => _closedList.Contains(state) || _openList.Contains(state);
    }
}
