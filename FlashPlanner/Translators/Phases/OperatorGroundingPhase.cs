using FlashPlanner.Models;
using FlashPlanner.Models.SAS;
using FlashPlanner.Translators.Helpers;
using FlashPlanner.Translators.Normalizers;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Overloads;
using PDDLSharp.Translators.Grounders;
using System.Diagnostics;

namespace FlashPlanner.Translators.Phases
{
    public class OperatorGroundingPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public ParametizedGrounder Grounder;
        public NodeNormalizer Normalizer;

        public OperatorGroundingPhase(LogEventHandler? doLog, ParametizedGrounder grounder, NodeNormalizer normalizer)
        {
            DoLog = doLog;
            Grounder = grounder;
            Normalizer = normalizer;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Normalizing actions...");
            var normalizedActions = NormalizeActions(from.PDDL.Domain.Actions);
            DoLog?.Invoke($"A total of {normalizedActions.Count} normalized actions to ground.");
            DoLog?.Invoke($"Grounding operators...");
            var operators = GetOperators(normalizedActions, from.PDDL);
            DoLog?.Invoke($"A total of {operators.Count} operators have been made.");
            from.SAS = new SASDecl(operators, from.SAS.Goal, from.SAS.Init, from.SAS.Facts);
            return from;
        }

        private List<ActionDecl> NormalizeActions(List<ActionDecl> actions)
        {
            var normalizedActions = new List<ActionDecl>();
            int count = 1;
            foreach (var action in actions)
            {
                if (Abort) return new List<ActionDecl>();
                DoLog?.Invoke($"Normalizing action '{action.Name}' [{count++} of {actions.Count}]");
                action.EnsureAnd();
                normalizedActions.AddRange(Normalizer.DeconstructAction(action));
            }
            return normalizedActions;
        }

        private List<Operator> GetOperators(List<ActionDecl> actions, PDDLDecl decl)
        {
            var operators = new List<Operator>();
            foreach (var action in actions)
            {
                DoLog?.Invoke($"Grounding action '{action.Name}'...");
                var newActs = Grounder.Ground(action).Cast<ActionDecl>();
                DoLog?.Invoke($"Action '{action.Name}' created {newActs.Count()} operators.");
                if (Abort) return new List<Operator>();
                foreach (var act in newActs)
                {
                    if (Abort) return new List<Operator>();
                    var preFacts = FactHelpers.ExtractFactsFromExp(act.Preconditions);
                    if (preFacts[true].Intersect(preFacts[false]).Any())
                        continue;
                    var pre = preFacts[true];

                    var effFacts = FactHelpers.ExtractFactsFromExp(act.Effects);
                    var add = effFacts[true];
                    var del = effFacts[false];

                    if (preFacts[false].Count > 0)
                    {
                        foreach (var fact in preFacts[false])
                        {
                            var nFact = FactHelpers.GetNegatedOf(fact);
                            pre.Add(nFact);

                            bool addToAdd = false;
                            bool addToDel = false;
                            if (add.Contains(fact))
                                addToDel = true;
                            if (del.Contains(fact))
                                addToAdd = true;

                            if (addToAdd)
                                add.Add(nFact);
                            if (addToDel)
                                del.Add(nFact);
                        }
                    }

                    var args = new List<string>();
                    foreach (var arg in act.Parameters.Values)
                        args.Add(arg.Name);

                    var newOp = new Operator(act.Name, args.ToArray(), pre.Distinct().ToArray(), add.Distinct().ToArray(), del.Distinct().ToArray(), 0);
                    operators.Add(newOp);
                }
            }

            return operators;
        }
    }
}
