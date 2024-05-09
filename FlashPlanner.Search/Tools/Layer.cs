﻿using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search.Tools
{
    public class Layer
    {
        public List<Operator> Operators { get; set; }
        public HashSet<Fact> Propositions { get; set; }

        public Layer(List<Operator> actions, HashSet<Fact> propositions)
        {
            Operators = actions;
            Propositions = propositions;
        }

        public Layer()
        {
            Operators = new List<Operator>();
            Propositions = new HashSet<Fact>();
        }

        public override string? ToString()
        {
            return $"Ops: {Operators.Count}, Props: {Propositions.Count}";
        }
    }
}
