using FlashPlanner.Models;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Overloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Translators.Phases
{
    public class ForceNonEqualParametersPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public ForceNonEqualParametersPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Forcing non-equality predicate on all parameter combinations...");
            foreach (var action in from.PDDL.Domain.Actions)
            {
                action.EnsureAnd();
                if (action.Preconditions is AndExp and)
                    for (int i = 0; i < action.Parameters.Values.Count; i++)
                        for (int j = i + 1; j < action.Parameters.Values.Count; j++)
                            and.Add(GenerateNotPredicateEq(action.Parameters.Values[i], action.Parameters.Values[j], and));
            }

            return from;
        }

        private NotExp GenerateNotPredicateEq(NameExp x, NameExp y, INode parent)
        {
            var args = new List<NameExp>()
            {
                x, y
            };
            var notNode = new NotExp(parent);
            notNode.Child = new PredicateExp(notNode, "=", args);
            return notNode;
        }
    }
}
