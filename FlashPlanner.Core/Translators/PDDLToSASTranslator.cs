﻿using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.Translators.Exceptions;
using FlashPlanner.Core.Translators.Normalizers;
using FlashPlanner.Core.Translators.Phases;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Translators.Grounders;
using System.Diagnostics;

namespace FlashPlanner.Core.Translators
{
    /// <summary>
    /// Primary translator for FlashPlanner
    /// </summary>
    public class PDDLToSASTranslator : LimitedComponent, ITranslator
    {
        /// <summary>
        /// Logging event for the front end
        /// </summary>
        public override event LogEventHandler? DoLog;

        /// <summary>
        /// If a (not (= ?x ?y)) should be added to all actions.
        /// </summary>
        public bool AssumeNoEqualParameters { get; set; } = false;

        private ParametizedGrounder? _grounder;
        private NodeNormalizer? _normalizer;
        private ITranslatorPhase? _currentPhase;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="assumeNoEqualParameters"></param>
        public PDDLToSASTranslator(bool assumeNoEqualParameters)
        {
            AssumeNoEqualParameters = assumeNoEqualParameters;
        }

        /// <summary>
        /// Optional override if anything extra needs to be aborted in the different implementation.
        /// </summary>
        public override void DoAbort()
        {
            _grounder?.Abort();
            _normalizer?.Abort();
            if (_currentPhase != null)
                _currentPhase.Abort = true;
        }

        private List<ITranslatorPhase> GetTranslatorPhases(PDDLDecl from)
        {
            _grounder = new ParametizedGrounder(from);
            _grounder.RemoveStaticsFromOutput = true;
            _normalizer = new NodeNormalizer(_grounder);

            var phases = new List<ITranslatorPhase>();
            phases.Add(new RemoveUnreachablePredicatesPhase(DoLog));
            if (AssumeNoEqualParameters)
                phases.Add(new ForceNonEqualParametersPhase(DoLog));
            phases.Add(new ContextualizationPhase(DoLog));
            phases.Add(new EnsureNoDuplicateExpressionsPhase(DoLog));
            phases.Add(new ExtractInitialFactsPhase(DoLog, _grounder, _normalizer));
            phases.Add(new ExtractGoalFactsPhase(DoLog, _grounder, _normalizer));
            phases.Add(new OperatorGroundingPhase(DoLog, _grounder, _normalizer));
            phases.Add(new ProcessNegativeFactsPhase(DoLog));
            phases.Add(new RemoveUselessOperatorsPhase(DoLog));
            phases.Add(new NormalizeOperatorIDsPhase(DoLog));
            phases.Add(new NormalizeFactIDsPhase(DoLog));
            phases.Add(new ResetOperatorsPhase(DoLog));
            phases.Add(new RemoveUnreachableOperatorsPhase(DoLog));
            phases.Add(new CheckIfClearlyUnsolvablePhase(DoLog));
            phases.Add(new NormalizeOperatorIDsPhase(DoLog));
            phases.Add(new NormalizeFactIDsPhase(DoLog));
            phases.Add(new ResetOperatorsPhase(DoLog));
            phases.Add(new GenerateFactHashesPhase(DoLog));
            phases.Add(new GenerateApplicabilityGraphPhase(DoLog));
            phases.Add(new GenerateBitMaskBoundsPhase(DoLog));

            return phases;
        }

        /// <summary>
        /// Convert a <seealso cref="PDDLDecl"/> into a <seealso cref="SASDecl"/>
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public TranslatorContext Translate(PDDLDecl from)
        {
            Start();

            DoLog?.Invoke($"Checking if task can be translated...");
            CheckIfValid(from);

            var phases = GetTranslatorPhases(from);
            var result = new TranslatorContext() { PDDL = from };
            foreach (var phase in phases)
            {
                if (Abort) return new TranslatorContext();
                _currentPhase = phase;
                var watch = new Stopwatch();
                watch.Start();
                result = phase.ExecutePhase(result);
                watch.Stop();
                DoLog?.Invoke($"\tTook {Math.Round(watch.Elapsed.TotalSeconds, 2)}s");
            }
            Stop();
            return result;
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
