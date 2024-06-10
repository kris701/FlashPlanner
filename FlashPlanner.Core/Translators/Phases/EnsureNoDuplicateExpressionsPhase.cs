using FlashPlanner.Core.Models;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Overloads;

namespace FlashPlanner.Core.Translators.Phases
{
    /// <summary>
    /// Ensure that there are no duplicate actions.
    /// Also make sure that all actions preconditions and effects are an <seealso cref="AndExp"/>
    /// </summary>
    public class EnsureNoDuplicateExpressionsPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public EnsureNoDuplicateExpressionsPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Ensuring that action effects and preconditions contains no duplicates...");
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
