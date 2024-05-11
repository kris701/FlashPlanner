using FlashPlanner.Heuristics;
using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;
using System.Diagnostics;
using System.Timers;

namespace FlashPlanner.Search.Classical
{
    public abstract class BaseClassicalSearch : IPlanner
    {
        public SASDecl Declaration { get; internal set; }
        public int Generated { get; internal set; }
        public int Expanded { get; internal set; }
        public int Evaluations => Heuristic.Evaluations;

        public bool Aborted { get; internal set; }
        public IHeuristic Heuristic { get; }
        public TimeSpan SearchTime => _searchWatch.Elapsed;
        public TimeSpan TimeLimit { get; set; } = TimeSpan.FromMinutes(30);

        internal HashSet<StateMove> _closedList = new HashSet<StateMove>();
        internal RefPriorityQueue _openList = new RefPriorityQueue();
        private System.Timers.Timer _timeoutTimer = new System.Timers.Timer();
        private Stopwatch _searchWatch = new Stopwatch();

        public BaseClassicalSearch(SASDecl decl, IHeuristic heuristic)
        {
            Declaration = decl;
            Heuristic = heuristic;
            SetupTimers();
        }

        private void SetupTimers()
        {
            _timeoutTimer = new System.Timers.Timer();
            _timeoutTimer.Interval = TimeLimit.TotalMilliseconds;
            _timeoutTimer.Elapsed += OnTimedOut;
            _timeoutTimer.AutoReset = false;
        }

        public ActionPlan Solve()
        {
            var state = new SASStateSpace(Declaration);
            if (state.IsInGoal())
                return new ActionPlan(new List<GroundedAction>());

            _closedList = new HashSet<StateMove>();
            _openList = InitializeQueue(Heuristic, state, Declaration.Operators);

            Expanded = 0;
            Generated = 0;
            Heuristic.Reset();

            SetupTimers();
            _timeoutTimer.Start();
            _searchWatch.Start();

            var result = Solve(Heuristic, state);

            _searchWatch.Stop();
            _timeoutTimer.Stop();

            if (result == null)
                return new ActionPlan();

            return result;
        }

        private void OnTimedOut(object? source, ElapsedEventArgs e)
        {
            Aborted = true;
        }

        internal SASStateSpace GenerateNewState(SASStateSpace state, Operator op)
        {
            Generated++;
            var newState = new SASStateSpace(state);
            newState.Execute(op);
            return newState;
        }

        internal RefPriorityQueue InitializeQueue(IHeuristic h, SASStateSpace state, List<Operator> operators)
        {
            var queue = new RefPriorityQueue();
            var fromMove = new StateMove();
            fromMove.hValue = int.MaxValue;
            var hValue = h.GetValue(fromMove, state, operators);
            queue.Enqueue(new StateMove(state, hValue), hValue);
            return queue;
        }

        internal StateMove ExpandBestState(RefPriorityQueue? from = null)
        {
            if (from == null)
                from = _openList;
            var stateMove = from.Dequeue();
            _closedList.Add(stateMove);
            Expanded++;
            return stateMove;
        }

        internal List<GroundedAction> GeneratePlanChain(List<Operator> steps, Operator newOp)
        {
            var chain = new List<GroundedAction>();

            chain.AddRange(GeneratePlanChain(steps));
            chain.Add(GenerateFromOp(newOp));

            return chain;
        }

        internal List<GroundedAction> GeneratePlanChain(List<Operator> steps)
        {
            var chain = new List<GroundedAction>();

            foreach (var step in steps)
                chain.Add(GenerateFromOp(step));

            return chain;
        }

        internal GroundedAction GenerateFromOp(Operator op) => new GroundedAction(op.Name, op.Arguments);

        internal abstract ActionPlan? Solve(IHeuristic h, SASStateSpace state);

        public virtual void Dispose()
        {
            _closedList.Clear();
            _closedList.EnsureCapacity(0);
            _openList.Clear();
            _openList.EnsureCapacity(0);

            _timeoutTimer.Stop();

            GC.SuppressFinalize(this);
        }
    }
}
