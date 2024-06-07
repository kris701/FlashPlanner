using FlashPlanner.Models;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Models.PDDL.Overloads;

namespace FlashPlanner.Translators.Phases
{
    public class EnsureNoDuplicateExpressionsPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public EnsureNoDuplicateExpressionsPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Ensureing that action effects and preconditions contains no duplicates...");
            foreach (var act in from.PDDL.Domain.Actions)
            {
                act.EnsureAnd();
                if (act.Preconditions is AndExp pres)
                    pres.Children = pres.Children.Distinct().ToList();
                if (act.Effects is AndExp effs)
                    effs.Children = effs.Children.Distinct().ToList();
            }
            return from;
        }
    }
}
