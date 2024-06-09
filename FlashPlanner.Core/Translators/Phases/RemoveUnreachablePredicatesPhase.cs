using FlashPlanner.Models.SAS;
using FlashPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Domain;

namespace FlashPlanner.Translators.Phases
{
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
