using FlashPlanner.Heuristics;
using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search.Classical
{
    public abstract class BaseClassicalSearch : LimitedComponent, IPlanner
    {
        public override event LogEventHandler? DoLog;

        public SASDecl Declaration { get; internal set; }
        public int Generated { get; internal set; }
        public int Expanded { get; internal set; }
        public int Evaluations => Heuristic.Evaluations;

        public IHeuristic Heuristic { get; }

        internal HashSet<StateMove> _closedList = new HashSet<StateMove>();
        internal RefPriorityQueue _openList = new RefPriorityQueue();

        public BaseClassicalSearch(SASDecl decl, IHeuristic heuristic)
        {
            Declaration = decl;
            Heuristic = heuristic;
        }

        public ActionPlan Solve()
        {
            var state = new SASStateSpace(Declaration);
            if (state.IsInGoal())
                return new ActionPlan(new List<GroundedAction>());

            Start();

            _closedList = new HashSet<StateMove>();
            _openList = InitializeQueue(Heuristic, state, Declaration.Operators);

            Expanded = 0;
            Generated = 0;
            Heuristic.Reset();

            var result = Solve(Heuristic, state);

            Stop();

            if (result == null)
                return new ActionPlan();

            return result;
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

            GC.SuppressFinalize(this);
        }
    }
}
