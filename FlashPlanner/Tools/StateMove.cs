using FlashPlanner.States;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Tools
{
    /// <summary>
    /// Represents a state during the search, with the actual state space, what operators have been executed and the heuristic value.
    /// </summary>
    public class StateMove
    {
        /// <summary>
        /// The current state space
        /// </summary>
        public SASStateSpace State { get; private set; }
        /// <summary>
        /// Steps that have been taken so far
        /// </summary>
        public List<int> PlanSteps { get; set; }
        /// <summary>
        /// Heuristic value of this state
        /// </summary>
        public int hValue { get; set; }

        /// <summary>
        /// Initial state constructor
        /// </summary>
        /// <param name="state"></param>
        public StateMove(SASStateSpace state)
        {
            State = state;
            PlanSteps = new List<int>();
            hValue = int.MaxValue;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="move"></param>
        public StateMove(StateMove move, Operator op)
        {
            State = new SASStateSpace(move.State, op);
            PlanSteps = new List<int>(move.PlanSteps);
            PlanSteps.Add(op.ID);
            hValue = -1;
        }

        /// <summary>
        /// Gets the hashcode of the <seealso cref="State"/>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => State.GetHashCode();

        /// <summary>
        /// Checks if the <seealso cref="State"/> objects are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (obj is StateMove move)
                return move.State.Equals(State);
            return false;
        }

        /// <summary>
        /// Nicer to look at tostring implementation
        /// </summary>
        /// <returns></returns>
        public override string? ToString()
        {
            return $"Steps: {PlanSteps.Count}, h: {hValue}, State Size: {State.Count}";
        }
    }
}
