using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.Translators.Exceptions;
using FlashPlanner.Core.Translators.Helpers;
using FlashPlanner.Core.Translators.Normalizers;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Translators.Grounders;

namespace FlashPlanner.Core.Translators.Phases
{
    /// <summary>
    /// Extract goal <seealso cref="Fact"/>s from the <seealso cref="PDDLDecl.Problem"/>
    /// </summary>
    public class ExtractGoalFactsPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public ParametizedGrounder Grounder;
        public NodeNormalizer Normalizer;

        public ExtractGoalFactsPhase(LogEventHandler? doLog, ParametizedGrounder grounder, NodeNormalizer normalizer)
        {
            DoLog = doLog;
            Grounder = grounder;
            Normalizer = normalizer;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Extracting goal facts...");
            var goals = new HashSet<Fact>();
            if (from.PDDL.Problem.Goal != null)
                goals = ExtractGoalFacts(from.PDDL.Problem.Goal.GoalExp, from);
            DoLog?.Invoke($"A total of {goals.Count} goal facts have been extracted.");
            from.SAS = new SASDecl(from.SAS.Operators, goals.ToArray(), from.SAS.Init, from.SAS.Facts);
            return from;
        }

        private HashSet<Fact> ExtractGoalFacts(IExp goalExp, TranslatorContext from)
        {
            var normalized = Normalizer.Deconstruct(ExpressionHelper.EnsureAnd(goalExp));
            if (normalized.FindTypes<OrExp>().Count > 0)
                throw new TranslatorException("Translator does not support or expressions in goal declaration!");
            var goals = FactHelpers.ExtractFactsFromExp(normalized);
            var goal = goals[true];
            foreach (var fact in goals[false])
                goal.Add(FactHelpers.GetNegatedOf(fact));
            return goal.ToHashSet();
        }
    }
}
