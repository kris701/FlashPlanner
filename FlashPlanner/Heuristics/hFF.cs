﻿using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Heuristics
{
    public class hFF : BaseHeuristic
    {
        private readonly SASDecl _declaration;
        private OperatorRPG _graphGenerator;

        public hFF(SASDecl declaration)
        {
            _declaration = declaration;
            _graphGenerator = new OperatorRPG(declaration);
        }

        public override int GetValue(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            Evaluations++;
            var relaxedPlan = _graphGenerator.GenerateReplaxedPlan(
                state,
                operators);
            if (_graphGenerator.Failed)
                return int.MaxValue;
            return relaxedPlan.Count;
        }

        public override void Reset()
        {
            base.Reset();
            _graphGenerator = new OperatorRPG(_declaration);
        }
    }
}
