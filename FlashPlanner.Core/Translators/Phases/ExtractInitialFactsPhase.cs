using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.Translators.Helpers;
using FlashPlanner.Core.Translators.Normalizers;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkits;
using PDDLSharp.Translators.Grounders;

namespace FlashPlanner.Core.Translators.Phases
{
    public class ExtractInitialFactsPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public ParametizedGrounder Grounder;
        public NodeNormalizer Normalizer;

        public ExtractInitialFactsPhase(LogEventHandler? doLog, ParametizedGrounder grounder, NodeNormalizer normalizer)
        {
            DoLog = doLog;
            Grounder = grounder;
            Normalizer = normalizer;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Extracting init facts...");
            var inits = new HashSet<Fact>();
            if (from.PDDL.Problem.Init != null)
                inits = ExtractInitFacts(from.PDDL.Problem.Init.Predicates, from.PDDL);
            DoLog?.Invoke($"A total of {inits.Count} initial facts have been extracted.");
            from.SAS = new SASDecl(from.SAS.Operators, from.SAS.Goal, inits.ToArray(), from.SAS.Facts);
            return from;
        }

        private HashSet<Fact> ExtractInitFacts(List<IExp> inits, PDDLDecl decl)
        {
            var initFacts = new List<Fact>();
            var statics = SimpleStaticPredicateDetector.FindStaticPredicates(decl);

            var normalized = new List<IExp>();
            foreach (var exp in inits)
                normalized.Add(Normalizer.Deconstruct(exp));

            foreach (var init in normalized)
                if (init is PredicateExp pred)
                    initFacts.Add(FactHelpers.GetFactFromPredicate(pred));

            initFacts.RemoveAll(x => statics.Any(y => y.Name == x.Name));

            return initFacts.ToHashSet();
        }
    }
}
