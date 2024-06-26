﻿using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.Models
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
        /// How many steps that it took to get to this staet
        /// </summary>
        public int Steps { get; set; }
        /// <summary>
        /// What operator was executed to get here
        /// </summary>
        public uint Operator { get; set; }
        /// <summary>
        /// Heuristic value of this state
        /// </summary>
        public uint hValue { get; set; }

        /// <summary>
        /// Initial state constructor
        /// </summary>
        /// <param name="state"></param>
        public StateMove(SASStateSpace state)
        {
            State = state;
            hValue = uint.MaxValue;
            Steps = 0;
            Operator = 0;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="move"></param>
        /// <param name="op"></param>
        public StateMove(StateMove move, Operator op)
        {
            State = new SASStateSpace(move.State, op);
            Steps = move.Steps + 1;
            hValue = uint.MaxValue;
            Operator = op.ID;
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
            return $"h: {hValue}, State Size: {State.Count}";
        }
    }
}
