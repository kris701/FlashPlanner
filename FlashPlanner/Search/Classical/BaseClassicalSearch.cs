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

        internal StateMove GenerateNewState(StateMove state, Operator op)
        {
            Generated++;
            var stateMove = new StateMove(state);
            stateMove.State.Execute(op);
            stateMove.PlanSteps.Add(op.ID);
            return stateMove;
        }

        internal RefPriorityQueue InitializeQueue(IHeuristic h, SASStateSpace state, List<Operator> operators)
        {
            var queue = new RefPriorityQueue();
            var fromMove = new StateMove(state);
            queue.Enqueue(fromMove, int.MaxValue);
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

        internal ActionPlan GeneratePlanChain(StateMove state)
        {
            var chain = new List<GroundedAction>();

            foreach (var step in state.PlanSteps)
                chain.Add(GenerateFromOp(Declaration.Operators.First(x => x.ID == step)));

            return new ActionPlan(chain);
        }

        internal GroundedAction GenerateFromOp(Operator op) => new GroundedAction(op.Name, op.Arguments);

        internal abstract ActionPlan? Solve(IHeuristic h, SASStateSpace state);

        internal bool IsVisited(StateMove state) => _closedList.Contains(state) || _openList.Contains(state);

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
