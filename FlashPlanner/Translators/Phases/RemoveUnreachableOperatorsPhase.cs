﻿using FlashPlanner.Heuristics;
using FlashPlanner.Models;
using FlashPlanner.Models.SAS;
using FlashPlanner.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Translators.Phases
{
    public class RemoveUnreachableOperatorsPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public RemoveUnreachableOperatorsPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Checking if all {from.SAS.Operators.Count} operators are reachable.");
            var opsNow = GetPotentiallyReachableOperators(from.SAS);
            DoLog?.Invoke($"{from.SAS.Operators.Count - opsNow.Count} operators removed by not being reachable.");
            from.SAS = new SASDecl(opsNow, from.SAS.Goal, from.SAS.Init, from.SAS.Facts);
            return from;
        }

        private List<Operator> GetPotentiallyReachableOperators(SASDecl decl)
        {
            bool any = true;
            bool[] covered = new bool[decl.Operators.Count];
            var state = new RelaxedSASStateSpace(new Models.TranslatorContext(decl, new PDDLSharp.Models.PDDL.PDDLDecl(), new int[decl.Facts]));
            var applicables = new List<Operator>();
            while (any)
            {
                any = false;
                var applicableNow = new List<Operator>();
                for (int i = 0; i < decl.Operators.Count; i++)
                {
                    if (!covered[i] && state.IsApplicable(decl.Operators[i]))
                    {
                        any = true;
                        covered[i] = true;
                        applicableNow.Add(decl.Operators[i]);
                        applicables.Add(decl.Operators[i]);
                    }
                }

                state = new RelaxedSASStateSpace(state, applicableNow);
            }

            return applicables;
        }
    }
}
