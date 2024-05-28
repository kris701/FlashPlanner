using FlashPlanner.Heuristics;
using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search
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

        internal SASDecl _declaration;
        internal HashSet<StateMove> _closedList = new HashSet<StateMove>();
        internal RefPriorityQueue<StateMove> _openList = new RefPriorityQueue<StateMove>();

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        public BaseHeuristicPlanner(IHeuristic heuristic)
        {
            Heuristic = heuristic;
            _declaration = new SASDecl();
        }

        /// <summary>
        /// Get a plan for some <seealso cref="SASDecl"/> on the <seealso cref="IHeuristic"/> provided
        /// </summary>
        /// <param name="decl"></param>
        /// <returns>A plan or null if unsolvable</returns>
        public ActionPlan? Solve(SASDecl decl)
        {
            Heuristic.Reset();
            _declaration = decl;

            var state = new SASStateSpace(_declaration);
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
            var stateMove = new StateMove(state);
            stateMove.State.Execute(op);
            stateMove.PlanSteps.Add(op.ID);
            return stateMove;
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

            foreach (var step in state.PlanSteps)
                chain.Add(GenerateFromOp(_declaration.Operators.First(x => x.ID == step)));

            return new ActionPlan(chain);
        }

        internal GroundedAction GenerateFromOp(Operator op) => new GroundedAction(op.Name, op.Arguments);

        internal bool IsVisited(StateMove state) => _closedList.Contains(state) || _openList.Contains(state);
    }
}
