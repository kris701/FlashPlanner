﻿using FlashPlanner.Translators.Components;
using FlashPlanner.Translators.Exceptions;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Overloads;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.SAS;
using PDDLSharp.Tools;
using PDDLSharp.Translators.Grounders;
using System.Diagnostics;
using System.Timers;

namespace FlashPlanner.Translators
{
    /// <summary>
    /// Primary translator for FlashPlanner
    /// </summary>
    public class PDDLToSASTranslator : ITranslator
    {
        /// <summary>
        /// A bool representing if statics should be removed from the resulting <seealso cref="SASDecl"/>
        /// </summary>
        public bool RemoveStaticsFromOutput { get; set; } = false;
        /// <summary>
        /// Time limit for the translation
        /// </summary>
        public TimeSpan TimeLimit { get; set; } = TimeSpan.FromMinutes(30);
        /// <summary>
        /// Time it took to translate
        /// </summary>
        public TimeSpan TranslationTime { get; internal set; }
        /// <summary>
        /// Aborted is true if the time limit was reached
        /// </summary>
        public bool Aborted { get; internal set; }
        /// <summary>
        /// How many facts have been created during the translation
        /// </summary>
        public int Facts { get; internal set; }
        /// <summary>
        /// How many operators have been created during the translation
        /// </summary>
        public int Operators { get; internal set; }

        private ParametizedGrounder? _grounder;
        private NodeDeconstructor? _deconstructor;
        private List<Fact> _factSet = new List<Fact>();
        private int _factID = 0;
        private int _opID = 0;
        private List<Fact> _negativeFacts = new List<Fact>();
        private readonly string _negatedPrefix = "$neg-";

        /// <summary>
        /// Constructor that can take in a bool saying if statics should be removed from the output
        /// </summary>
        /// <param name="removeStaticsFromOutput"></param>
        public PDDLToSASTranslator(bool removeStaticsFromOutput = false)
        {
            RemoveStaticsFromOutput = removeStaticsFromOutput;
        }

        private System.Timers.Timer GetTimer(TimeSpan interval)
        {
            System.Timers.Timer newTimer = new System.Timers.Timer();
            newTimer.Interval = interval.TotalMilliseconds;
            newTimer.Elapsed += OnTimedOut;
            newTimer.AutoReset = false;
            return newTimer;
        }

        private void OnTimedOut(object? source, ElapsedEventArgs e)
        {
            Aborted = true;
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
            if (!from.IsContextualised)
            {
                var listener = new ErrorListener();
                var contextualiser = new PDDLContextualiser(listener);
                contextualiser.Contexturalise(from);
            }

            Aborted = true;
            CheckIfValid(from);
            Aborted = false;

            var timer = GetTimer(TimeLimit);
            timer.Start();
            var watch = new Stopwatch();
            watch.Start();

            var domainVariables = new HashSet<string>();
            var operators = new List<Operator>();
            var goals = new List<Fact>();
            var inits = new List<Fact>();

            var grounder = new ParametizedGrounder(from);
            _grounder = grounder;
            grounder.RemoveStaticsFromOutput = RemoveStaticsFromOutput;
            var deconstructor = new NodeDeconstructor(grounder);
            _deconstructor = deconstructor;
            _factID = 0;
            _factSet = new List<Fact>();
            _opID = 0;
            _negativeFacts = new List<Fact>();

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
            if (Aborted) return new SASDecl();

            // Goal
            if (from.Problem.Goal != null)
                goals = ExtractGoalFacts(from.Problem.Goal.GoalExp, deconstructor);
            if (Aborted) return new SASDecl();

            // Operators
            operators = GetOperators(from, grounder, deconstructor);
            if (Aborted) return new SASDecl();

            // Handle negative preconditions, if there where any
            if (_negativeFacts.Count > 0)
            {
                var negInits = ProcessNegativeFactsInOperators(operators);
                inits = ProcessNegativeFactsInInit(negInits, inits);
            }

            var result = new SASDecl(domainVariables, operators, goals.ToHashSet(), inits.ToHashSet());

            // Only use operators that is reachable in a relaxed planning graph
            var reachability = new ReachabilityChecker();
            result.Operators = reachability.GetPotentiallyReachableOperators(result);

            // Check if operators can satisfy goal condition
            foreach (var goal in goals)
                if (!result.Operators.Any(x => x.Add.Contains(goal)))
                    result.Operators.Clear();

            watch.Stop();
            timer.Stop();
            TranslationTime = watch.Elapsed;
            return result;
        }

        private List<Fact> ProcessNegativeFactsInOperators(List<Operator> operators)
        {
            var negInits = new List<Fact>();
            // Adds negated facts to all ops
            foreach (var fact in _negativeFacts)
            {
                for (int i = 0; i < operators.Count; i++)
                {
                    var negDels = operators[i].Add.Where(x => x.Name == fact.Name).ToList();
                    var negAdds = operators[i].Del.Where(x => x.Name == fact.Name).ToList();
                    negInits.AddRange(operators[i].Pre.Where(x => x.Name.Contains(_negatedPrefix) && x.Name.Replace(_negatedPrefix, "") == fact.Name).ToHashSet());

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
                if (Aborted) return new List<Fact>();
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
                if (Aborted) return new List<Fact>();
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

        private List<Fact> ExtractInitFacts(List<IExp> inits, NodeDeconstructor deconstructor)
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

        private List<Fact> ExtractGoalFacts(IExp goalExp, NodeDeconstructor deconstructor)
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

        private List<Operator> GetOperators(PDDLDecl decl, IGrounder<IParametized> grounder, NodeDeconstructor deconstructor)
        {
            var operators = new List<Operator>();
            foreach (var action in decl.Domain.Actions)
            {
                action.EnsureAnd();
                if (Aborted) return new List<Operator>();
                var deconstructedActions = deconstructor.DeconstructAction(action);
                foreach (var deconstructed in deconstructedActions)
                {
                    if (Aborted) return new List<Operator>();
                    var newActs = grounder.Ground(deconstructed).Cast<ActionDecl>();
                    foreach (var act in newActs)
                    {
                        if (Aborted) return new List<Operator>();

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
                                if (!_negativeFacts.Any(x => x.Name == fact.Name))
                                    _negativeFacts.Add(fact);

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
            var find = _factSet.FirstOrDefault(x => x.ContentEquals(newFact));
            if (find == null)
            {
                newFact.ID = _factID++;
                Facts++;
                _factSet.Add(newFact);
            }
            else
                newFact.ID = find.ID;
            return newFact;
        }

        private Fact GetNegatedOf(Fact fact)
        {
            var newFact = new Fact($"{_negatedPrefix}{fact.Name}", fact.Arguments);
            var find = _factSet.FirstOrDefault(x => x.ContentEquals(newFact));
            if (find == null)
            {
                newFact.ID = _factID++;
                Facts++;
                _factSet.Add(newFact);
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
