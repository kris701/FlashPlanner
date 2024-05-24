using FlashPlanner.Translators.Exceptions;
using FlashPlanner.Translators.Normalizers;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Overloads;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.SAS;
using PDDLSharp.Translators.Grounders;

namespace FlashPlanner.Translators
{
    /// <summary>
    /// Primary translator for FlashPlanner
    /// </summary>
    public class PDDLToSASTranslator : LimitedComponent, ITranslator
    {
        public override event LogEventHandler? DoLog;

        /// <summary>
        /// A bool representing if statics should be removed from the resulting <seealso cref="SASDecl"/>
        /// </summary>
        public bool RemoveStaticsFromOutput { get; set; } = false;
        /// <summary>
        /// If a (not (= ?x ?y)) should be added to all actions.
        /// </summary>
        public bool AssumeNoEqualParameters { get; set; } = false;
        /// <summary>
        /// How many facts have been created during the translation
        /// </summary>
        public int Facts { get; internal set; }
        /// <summary>
        /// How many operators have been created during the translation
        /// </summary>
        public int Operators { get; internal set; }

        private ParametizedGrounder? _grounder;
        private NodeNormalizer? _deconstructor;
        private Dictionary<string, List<Fact>> _factSet = new Dictionary<string, List<Fact>>();
        private int _factID = 0;
        private int _opID = 0;
        private Dictionary<string, List<Fact>> _negativeFacts = new Dictionary<string, List<Fact>>();
        private readonly string _negatedPrefix = "$neg-";

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="removeStaticsFromOutput"></param>
        /// <param name="assumeNoEqualParameters"></param>
        public PDDLToSASTranslator(bool removeStaticsFromOutput, bool assumeNoEqualParameters)
        {
            RemoveStaticsFromOutput = removeStaticsFromOutput;
            AssumeNoEqualParameters = assumeNoEqualParameters;
        }

        public override void DoAbort()
        {
            if (_grounder != null)
                _grounder.Abort();
            if (_deconstructor != null)
                _deconstructor.Abort();
        }

        /// <summary>
        /// Convert a <seealso cref="PDDLDecl"/> into a <seealso cref="SASDecl"/>
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public SASDecl Translate(PDDLDecl from)
        {
            Start();

            if (AssumeNoEqualParameters)
                from.Domain.Actions = InsertNonEqualsInActions(from.Domain.Actions);

            if (!from.IsContextualised)
            {
                DoLog?.Invoke($"Contextualizing...");
                var listener = new ErrorListener();
                var contextualiser = new PDDLContextualiser(listener);
                contextualiser.Contexturalise(from);
            }

            DoLog?.Invoke($"Checking if task can be translated...");
            CheckIfValid(from);

            var domainVariables = new HashSet<string>();
            var operators = new List<Operator>();
            var goals = new List<Fact>();
            var inits = new List<Fact>();

            var grounder = new ParametizedGrounder(from);
            _grounder = grounder;
            grounder.RemoveStaticsFromOutput = RemoveStaticsFromOutput;
            var deconstructor = new NodeNormalizer(grounder);
            _deconstructor = deconstructor;
            _factID = 0;
            _factSet = new Dictionary<string, List<Fact>>();
            _negativeFacts = new Dictionary<string, List<Fact>>();

            if (from.Domain.Predicates != null)
            {
                foreach (var pred in from.Domain.Predicates.Predicates)
                {
                    _factSet.Add(pred.Name, new List<Fact>());
                    _negativeFacts.Add(pred.Name, new List<Fact>());
                }
            }
            DoLog?.Invoke($"Fact translation dictionary contains {_factSet.Count} keys");

            _opID = 0;

            // Domain variables
            if (from.Problem.Objects != null)
                foreach (var obj in from.Problem.Objects.Objs)
                    domainVariables.Add(obj.Name);
            if (from.Domain.Constants != null)
                foreach (var cons in from.Domain.Constants.Constants)
                    domainVariables.Add(cons.Name);

            // Init
            if (from.Problem.Init != null)
                inits = ExtractInitFacts(from.Problem.Init.Predicates, deconstructor);
            if (Abort) return new SASDecl();

            // Goal
            if (from.Problem.Goal != null)
                goals = ExtractGoalFacts(from.Problem.Goal.GoalExp, deconstructor);
            if (Abort) return new SASDecl();

            // Operators
            DoLog?.Invoke($"Normalizing actions...");
            var normalizedActions = NormalizeActions(from.Domain.Actions, deconstructor);
            DoLog?.Invoke($"A total of {normalizedActions.Count} normalized actions to ground.");
            DoLog?.Invoke($"Grounding operators...");
            operators = GetOperators(normalizedActions, grounder, deconstructor);
            if (Abort) return new SASDecl();

            // Handle negative preconditions, if there where any
            if (_negativeFacts.Any(x => x.Value.Count > 0))
            {
                DoLog?.Invoke($"Task contains negative facts! Ensuring operators upholds them...");
                var negInits = ProcessNegativeFactsInOperators(operators);
                inits = ProcessNegativeFactsInInit(negInits, inits);
            }

            var result = new SASDecl(domainVariables, operators, goals.ToHashSet(), inits.ToHashSet());

            // Only use operators that is reachable in a relaxed planning graph
            var reachability = new ReachabilityChecker();
            DoLog?.Invoke($"Checking if all {result.Operators.Count} operators are reachable.");
            var opsNow = reachability.GetPotentiallyReachableOperators(result);
            DoLog?.Invoke($"{result.Operators.Count - opsNow.Count} operators removed by not being reachable.");
            result.Operators = opsNow;

            // Check if operators can satisfy goal condition
            foreach (var goal in goals)
            {
                if (!result.Operators.Any(x => x.Add.Contains(goal)))
                {
                    result.Operators.Clear();
                    DoLog?.Invoke($"Goal fact '{goal}' cannot be reached! Removing all operators");
                }
            }

            Stop();
            return result;
        }

        private List<ActionDecl> InsertNonEqualsInActions(List<ActionDecl> actions)
        {
            foreach (var action in actions)
            {
                action.EnsureAnd();
                if (action.Preconditions is AndExp and)
                    for (int i = 0; i < action.Parameters.Values.Count; i++)
                        for (int j = i + 1; j < action.Parameters.Values.Count; j++)
                            and.Add(GenerateNotPredicateEq(action.Parameters.Values[i], action.Parameters.Values[j], and));
            }
            return actions;
        }

        private IExp GenerateNotPredicateEq(NameExp x, NameExp y, INode parent)
        {
            var args = new List<NameExp>()
            {
                x, y
            };
            var notNode = new NotExp(parent);
            notNode.Child = new PredicateExp(notNode, "=", args);
            return notNode;
        }

        private List<Fact> ProcessNegativeFactsInOperators(List<Operator> operators)
        {
            var negInits = new List<Fact>();
            // Adds negated facts to all ops
            foreach (var fact in _negativeFacts.Keys)
            {
                if (_negativeFacts[fact].Count == 0)
                    continue;
                for (int i = 0; i < operators.Count; i++)
                {
                    var negDels = operators[i].Add.Where(x => x.Name == fact).ToList();
                    var negAdds = operators[i].Del.Where(x => x.Name == fact).ToList();
                    negInits.AddRange(operators[i].Pre.Where(x => x.Name.Contains(_negatedPrefix) && x.Name.Replace(_negatedPrefix, "") == fact).ToHashSet());

                    if (negDels.Count == 0 && negAdds.Count == 0)
                        continue;

                    var adds = operators[i].Add.ToHashSet();
                    foreach (var nFact in negAdds)
                    {
                        var negated = GetNegatedOf(nFact);
                        negInits.Add(negated);
                        adds.Add(negated);
                    }
                    var dels = operators[i].Del.ToHashSet();
                    foreach (var nFact in negDels)
                    {
                        var negated = GetNegatedOf(nFact);
                        negInits.Add(negated);
                        dels.Add(negated);
                    }

                    if (adds.Count != operators[i].Add.Count() || dels.Count != operators[i].Del.Count())
                    {
                        var id = operators[i].ID;
                        operators[i] = new Operator(
                            operators[i].Name,
                            operators[i].Arguments,
                            operators[i].Pre,
                            adds.ToArray(),
                            dels.ToArray());
                        operators[i].ID = id;
                    }
                }
                if (Abort) return new List<Fact>();
            }
            return negInits;
        }

        private List<Fact> ProcessNegativeFactsInInit(List<Fact> negInits, List<Fact> init)
        {
            foreach (var fact in negInits)
            {
                var findTrue = new Fact(fact.Name.Replace(_negatedPrefix, ""), fact.Arguments);
                if (!init.Any(x => x.ContentEquals(findTrue)))
                    init.Add(fact);
                if (Abort) return new List<Fact>();
            }
            return init;
        }

        private Dictionary<bool, List<Fact>> ExtractFactsFromExp(IExp exp, bool possitive = true)
        {
            var facts = new Dictionary<bool, List<Fact>>();
            facts.Add(true, new List<Fact>());
            facts.Add(false, new List<Fact>());

            switch (exp)
            {
                case EmptyExp: break;
                case PredicateExp pred: facts[possitive].Add(GetFactFromPredicate(pred)); break;
                case NotExp not: facts = MergeDictionaries(facts, ExtractFactsFromExp(not.Child, !possitive)); break;
                case AndExp and:
                    foreach (var child in and.Children)
                        facts = MergeDictionaries(facts, ExtractFactsFromExp(child, possitive));
                    break;
                default:
                    throw new ArgumentException($"Cannot translate node type '{exp.GetType().Name}'");
            }

            return facts;
        }

        private Dictionary<bool, List<Fact>> MergeDictionaries(Dictionary<bool, List<Fact>> dict1, Dictionary<bool, List<Fact>> dict2)
        {
            var resultDict = new Dictionary<bool, List<Fact>>();
            foreach (var key in dict1.Keys)
                resultDict.Add(key, dict1[key]);
            foreach (var key in dict2.Keys)
                resultDict[key].AddRange(dict2[key]);

            return resultDict;
        }

        private List<Fact> ExtractInitFacts(List<IExp> inits, NodeNormalizer deconstructor)
        {
            var initFacts = new List<Fact>();

            var deconstructed = new List<IExp>();
            foreach (var exp in inits)
                deconstructed.Add(deconstructor.Deconstruct(exp));

            foreach (var init in deconstructed)
                if (init is PredicateExp pred)
                    initFacts.Add(GetFactFromPredicate(pred));

            return initFacts;
        }

        private List<Fact> ExtractGoalFacts(IExp goalExp, NodeNormalizer deconstructor)
        {
            var goal = new List<Fact>();
            var deconstructed = deconstructor.Deconstruct(EnsureAnd(goalExp));
            if (deconstructed.FindTypes<OrExp>().Count > 0)
                throw new TranslatorException("Translator does not support or expressions in goal declaration!");
            var goals = ExtractFactsFromExp(
                deconstructed);
            goal = goals[true];
            foreach (var fact in goals[false])
                goal.Add(GetNegatedOf(fact));
            return goal;
        }

        private List<ActionDecl> NormalizeActions(List<ActionDecl> actions, NodeNormalizer deconstructor)
        {
            var normalizedActions = new List<ActionDecl>();
            int count = 1;
            foreach (var action in actions)
            {
                DoLog?.Invoke($"Normalizing action '{action.Name}' [{count++} of {actions.Count}]");
                action.EnsureAnd();
                if (Abort) return new List<ActionDecl>();
                normalizedActions.AddRange(deconstructor.DeconstructAction(action));
            }
            return normalizedActions;
        }

        private List<Operator> GetOperators(List<ActionDecl> actions, IGrounder<IParametized> grounder, NodeNormalizer deconstructor)
        {
            var operators = new List<Operator>();
            foreach (var action in actions)
            {
                if (Abort) return new List<Operator>();
                var newActs = grounder.Ground(action).Cast<ActionDecl>();
                foreach (var act in newActs)
                {
                    if (Abort) return new List<Operator>();

                    var preFacts = ExtractFactsFromExp(act.Preconditions);
                    if (preFacts[true].Intersect(preFacts[false]).Count() > 0)
                        continue;
                    var pre = preFacts[true];

                    var effFacts = ExtractFactsFromExp(act.Effects);
                    var add = effFacts[true];
                    var del = effFacts[false];

                    if (preFacts[false].Count > 0)
                    {
                        foreach (var fact in preFacts[false])
                        {
                            if (_negativeFacts[fact.Name].Count == 0)
                                _negativeFacts[fact.Name].Add(fact);

                            var nFact = GetNegatedOf(fact);
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

                    var newOp = new Operator(act.Name, args.ToArray(), pre.ToArray(), add.ToArray(), del.ToArray());
                    newOp.ID = _opID++;
                    Operators++;
                    operators.Add(newOp);
                }
            }
            return operators;
        }

        private IExp EnsureAnd(IExp exp)
        {
            if (exp is AndExp)
                return exp;
            return new AndExp(new List<IExp>() { exp });
        }

        private Fact GetFactFromPredicate(PredicateExp pred)
        {
            var name = pred.Name;
            var args = new List<string>();
            foreach (var arg in pred.Arguments)
                args.Add(arg.Name);
            var newFact = new Fact(name, args.ToArray());
            var find = _factSet[name].FirstOrDefault(x => x.ContentEquals(newFact));
            if (find == null)
            {
                newFact.ID = _factID++;
                Facts++;
                _factSet[name].Add(newFact);
            }
            else
                newFact.ID = find.ID;
            return newFact;
        }

        private Fact GetNegatedOf(Fact fact)
        {
            var newFact = new Fact($"{_negatedPrefix}{fact.Name}", fact.Arguments);
            var find = _negativeFacts[fact.Name].FirstOrDefault(x => x.ContentEquals(newFact));
            if (find == null)
            {
                newFact.ID = _factID++;
                Facts++;
                _negativeFacts[fact.Name].Add(newFact);
            }
            else
                newFact.ID = find.ID;
            return newFact;
        }

        private void CheckIfValid(PDDLDecl decl)
        {
            if (decl.Domain.FindTypes<DerivedPredicateExp>().Count > 0 || decl.Problem.FindTypes<DerivedPredicateExp>().Count > 0)
                throw new TranslatorException("Translator does not support derived predicate nodes!");
            if (decl.Domain.FindTypes<DerivedDecl>().Count > 0 || decl.Problem.FindTypes<DerivedDecl>().Count > 0)
                throw new TranslatorException("Translator does not support derived decl nodes!");
            if (decl.Domain.FindTypes<TimedLiteralExp>().Count > 0 || decl.Problem.FindTypes<TimedLiteralExp>().Count > 0)
                throw new TranslatorException("Translator does not support Timed Literal nodes!");
            if (decl.Domain.FindTypes<NumericExp>().Count > 0 || decl.Problem.FindTypes<NumericExp>().Count > 0)
                throw new TranslatorException("Translator does not support Numeric nodes!");
            if (decl.Domain.FindTypes<LiteralExp>().Count > 0 || decl.Problem.FindTypes<LiteralExp>().Count > 0)
                throw new TranslatorException("Translator does not support Literal nodes!");
            if (decl.Domain.FindTypes<MetricDecl>().Count > 0 || decl.Problem.FindTypes<MetricDecl>().Count > 0)
                throw new TranslatorException("Translator does not support Metric nodes!");
            if (decl.Domain.FindTypes<SituationDecl>().Count > 0 || decl.Problem.FindTypes<SituationDecl>().Count > 0)
                throw new TranslatorException("Translator does not support Situation nodes!");
            if (decl.Domain.FindTypes<AxiomDecl>().Count > 0 || decl.Problem.FindTypes<AxiomDecl>().Count > 0)
                throw new TranslatorException("Translator does not support Axiom nodes!");
            if (decl.Domain.FindTypes<DurativeActionDecl>().Count > 0 || decl.Problem.FindTypes<DurativeActionDecl>().Count > 0)
                throw new TranslatorException("Translator does not support Durative Actions nodes!");
            if (decl.Domain.FindTypes<ExtendsDecl>().Count > 0 || decl.Problem.FindTypes<ExtendsDecl>().Count > 0)
                throw new TranslatorException("Translator does not support Extends nodes!");
            if (decl.Domain.FindTypes<TimelessDecl>().Count > 0 || decl.Problem.FindTypes<TimelessDecl>().Count > 0)
                throw new TranslatorException("Translator does not support Timeless nodes!");
        }
    }
}
