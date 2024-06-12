using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.Translators.Helpers;
using FlashPlanner.Core.Translators.Normalizers;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Overloads;
using PDDLSharp.Toolkits;
using PDDLSharp.Translators.Grounders;
using System;

namespace FlashPlanner.Core.Translators.Phases
{
    /// <summary>
    /// Grounding of <seealso cref="ActionDecl"/>s into <seealso cref="Operator"/>s.
    /// </summary>
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
            DoLog?.Invoke($"Grounding actions...");
            var operators = GroundActions(normalizedActions, from);
            DoLog?.Invoke($"A total of {operators.Count} operators have been made.");
            from.SAS = new SASDecl(operators, from.SAS.Goal, from.SAS.Init, from.SAS.Facts);
            return from;
        }

        /// <summary>
        /// Normalize actions.
        /// I.e. remove more advanced PDDL constructs suchs as forall expressions.
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Ground <seealso cref="ActionDecl"/>s into <seealso cref="Operator"/>s.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="decl"></param>
        /// <returns></returns>
        private List<Operator> GroundActions(List<ActionDecl> actions, TranslatorContext from)
        {
            var statics = SimpleStaticPredicateDetector.FindStaticPredicates(from.PDDL);
            var operators = new List<Operator>();
            foreach (var action in actions)
            {
                if (Abort) return new List<Operator>();
                var baseOp = GetBaseOperatorForAction(action, statics);

                DoLog?.Invoke($"Grounding action '{action.Name}'...");
                var permutations = Grounder.GetPermutations(action);
                foreach(var permutation in permutations)
                {
                    var newOp = baseOp.Copy();
                    var map = new Dictionary<string, string>();
                    for(int i = 0; i < baseOp.Arguments.Length; i++)
                        map.Add(baseOp.Arguments[i], Grounder.GetObjectFromIndex(permutation[i]));

                    for (int i = 0; i < newOp.Arguments.Length; i++)
                        newOp.Arguments[i] = map[newOp.Arguments[i]];
                    for(int i = 0; i < newOp.Pre.Length; i++)
                        for (int j = 0; j < newOp.Pre[i].Arguments.Length; j++)
                            if (newOp.Pre[i].Arguments[j].StartsWith('?'))
                                newOp.Pre[i].Arguments[j] = map[newOp.Pre[i].Arguments[j]];
                    for (int i = 0; i < newOp.Add.Length; i++)
                        for (int j = 0; j < newOp.Add[i].Arguments.Length; j++)
                            if (newOp.Add[i].Arguments[j].StartsWith('?'))
                                newOp.Add[i].Arguments[j] = map[newOp.Add[i].Arguments[j]];
                    for (int i = 0; i < newOp.Del.Length; i++)
                        for (int j = 0; j < newOp.Del[i].Arguments.Length; j++)
                            if (newOp.Del[i].Arguments[j].StartsWith('?'))
                                newOp.Del[i].Arguments[j] = map[newOp.Del[i].Arguments[j]];

                    operators.Add(newOp);
                }
                if (permutations.Count == 0)
                {
                    operators.Add(baseOp);
                    DoLog?.Invoke($"Action '{action.Name}' created 1 operator.");
                }
                else
                    DoLog?.Invoke($"Action '{action.Name}' created {permutations.Count} operators.");
            }

            return operators;
        }

        private Operator GetBaseOperatorForAction(ActionDecl action, List<PredicateExp> statics)
        {
            var preFacts = FactHelpers.ExtractFactsFromExp(action.Preconditions);
            var pre = preFacts[true];

            var effFacts = FactHelpers.ExtractFactsFromExp(action.Effects);
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
            foreach (var arg in action.Parameters.Values)
                args.Add(arg.Name);
            pre.RemoveAll(x => statics.Any(y => x.Name == y.Name));
            return new Operator(action.Name, args.ToArray(), pre.Distinct().ToArray(), add.Distinct().ToArray(), del.Distinct().ToArray(), 0);
        }
    }
}
