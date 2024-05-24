using FlashPlanner.States;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Tools
{
    public class StateMove
    {
        public SASStateSpace State { get; private set; }
        public List<int> PlanSteps { get; set; }
        public int hValue { get; set; }

        public StateMove(SASStateSpace state, int hvalue)
        {
            State = state;
            PlanSteps = new List<int>();
            hValue = hvalue;
        }

        public StateMove(SASStateSpace state) : this(state, -1)
        {
        }

        public StateMove() : this(new SASStateSpace(new SASDecl()), -1)
        {
        }

        public override int GetHashCode() => State.GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is StateMove move)
                return move.State.Equals(State);
            return false;
        }

        public override string? ToString()
        {
            return $"Steps: {PlanSteps.Count}, h: {hValue}, State Size: {State.Count}";
        }
    }
}
