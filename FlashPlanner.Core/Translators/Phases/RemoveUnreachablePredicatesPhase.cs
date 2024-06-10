using FlashPlanner.Core.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;

namespace FlashPlanner.Core.Translators.Phases
{
    /// <summary>
    /// Remove actions that for some reason, contains predicates that is never set in the init state, and is never set in effects of any actions either.
    /// </summary>
    public class RemoveUnreachablePredicatesPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public RemoveUnreachablePredicatesPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Checking if any precondition predicates are unreachable...");
            if (from.PDDL.Domain.Predicates != null && from.PDDL.Problem.Init != null)
            {
                var toRemove = new List<ActionDecl>();
                foreach (var pred in from.PDDL.Domain.Predicates.Predicates)
                {
                    if (from.PDDL.Problem.Init.Predicates.Any(x => x is PredicateExp pred2 && pred2.Name == pred.Name))
                        continue;
                    if (from.PDDL.Domain.Actions.Any(x => x.Effects.FindNames(pred.Name).Count > 0))
                        continue;
                    toRemove.AddRange(from.PDDL.Domain.Actions.Where(x => x.Preconditions.FindNames(pred.Name).Count > 0));
                }
                DoLog?.Invoke($"Removed {toRemove.Count} actions by being uncreachable by preconditions.");
                var cpy = from.PDDL.Copy();
                cpy.Domain.Actions.RemoveAll(x => toRemove.Any(y => x.Equals(y)));
                from = new TranslatorContext(from.SAS, cpy, from.FactHashes, from.ApplicabilityGraph);
            }
            return from;
        }
    }
}
