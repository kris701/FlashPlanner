using FlashPlanner.States;
using PDDLSharp.Models.SAS;


namespace FlashPlanner.Tools
{
    public class StateMove
    {
        public SASStateSpace State { get; private set; }
        public List<Operator> Steps { get; set; }
        public int hValue { get; set; }
        public bool Evaluated { get; set; } = true;

        public StateMove(SASStateSpace state, List<Operator> steps)
        {
            State = state;
            Steps = steps;
            hValue = -1;
        }

        public StateMove(SASStateSpace state, int hvalue)
        {
            State = state;
            Steps = new List<Operator>();
            hValue = hvalue;
        }

        public StateMove(SASStateSpace state)
        {
            State = state;
            Steps = new List<Operator>();
            hValue = -1;
        }

        public StateMove()
        {
            State = new SASStateSpace(new SASDecl());
            Steps = new List<Operator>();
            hValue = -1;
        }

        public override int GetHashCode()
        {
            return State.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is StateMove move)
                return move.State.Equals(State);
            return false;
        }

        public override string? ToString()
        {
            return $"Steps: {Steps.Count}, h: {hValue}, State Size: {State.Count}";
        }
    }
}
