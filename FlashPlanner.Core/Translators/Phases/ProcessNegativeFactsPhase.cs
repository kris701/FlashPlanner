using FlashPlanner.Models;
using FlashPlanner.Models.SAS;
using FlashPlanner.Translators.Helpers;

namespace FlashPlanner.Translators.Phases
{
    public class ProcessNegativeFactsPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public ProcessNegativeFactsPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Checking if negative facts are present...");
            var negativeFacts = FindNegativeFacts(from.SAS);
            if (negativeFacts.Count > 0)
            {
                DoLog?.Invoke($"Negative facts detected! Processing...");
                var negInits = ProcessNegativeFactsInOperators(from.SAS.Operators, negativeFacts);
                var inits = ProcessNegativeFactsInInit(negInits, from.SAS.Init);
                from.SAS = new SASDecl(from.SAS.Operators, from.SAS.Goal, inits.ToArray(), from.SAS.Facts);
            }

            return from;
        }

        private List<string> FindNegativeFacts(SASDecl decl)
        {
            var factsNames = new HashSet<string>();

            foreach (var fact in decl.Init)
                if (fact.Name.StartsWith(FactHelpers.NegatedPrefix))
                    factsNames.Add(fact.Name);
            foreach (var fact in decl.Goal)
                if (fact.Name.StartsWith(FactHelpers.NegatedPrefix))
                    factsNames.Add(fact.Name);
            foreach (var op in decl.Operators)
            {
                foreach (var fact in op.Pre)
                    if (fact.Name.StartsWith(FactHelpers.NegatedPrefix))
                        factsNames.Add(fact.Name);
                foreach (var fact in op.Add)
                    if (fact.Name.StartsWith(FactHelpers.NegatedPrefix))
                        factsNames.Add(fact.Name);
                foreach (var fact in op.Del)
                    if (fact.Name.StartsWith(FactHelpers.NegatedPrefix))
                        factsNames.Add(fact.Name);
            }

            var negFacts = new List<string>();
            foreach (var name in factsNames)
                negFacts.Add(name.Replace(FactHelpers.NegatedPrefix, ""));

            return negFacts;
        }

        private List<Fact> ProcessNegativeFactsInOperators(List<Operator> operators, List<string> negFacts)
        {
            var negInits = new List<Fact>();
            // Adds negated facts to all ops
            foreach (var fact in negFacts)
            {
                if (Abort) return new List<Fact>();
                for (int i = 0; i < operators.Count; i++)
                {
                    if (Abort) return new List<Fact>();
                    var negDels = operators[i].Add.Where(x => x.Name == fact).ToList();
                    var negAdds = operators[i].Del.Where(x => x.Name == fact).ToList();
                    negInits.AddRange(operators[i].Pre.Where(x => x.Name.Contains(FactHelpers.NegatedPrefix) && x.Name.Replace(FactHelpers.NegatedPrefix, "") == fact).ToHashSet());

                    if (negDels.Count == 0 && negAdds.Count == 0)
                        continue;

                    var adds = operators[i].Add.ToHashSet();
                    foreach (var nFact in negAdds)
                    {
                        var negated = FactHelpers.GetNegatedOf(nFact);
                        negInits.Add(negated);
                        adds.Add(negated);
                    }
                    var dels = operators[i].Del.ToHashSet();
                    foreach (var nFact in negDels)
                    {
                        var negated = FactHelpers.GetNegatedOf(nFact);
                        negInits.Add(negated);
                        dels.Add(negated);
                    }

                    if (adds.Count != operators[i].Add.Length || dels.Count != operators[i].Del.Length)
                    {
                        var id = operators[i].ID;
                        operators[i] = new Operator(
                            operators[i].Name,
                            operators[i].Arguments,
                            operators[i].Pre,
                            adds.ToArray(),
                            dels.ToArray(),
                            0);
                        operators[i].ID = id;
                    }
                }
            }
            return negInits;
        }

        private List<Fact> ProcessNegativeFactsInInit(List<Fact> negInits, Fact[] init)
        {
            var newInits = new HashSet<Fact>(init);
            foreach (var fact in negInits)
            {
                var findTrue = new Fact(fact.Name.Replace(FactHelpers.NegatedPrefix, ""), fact.Arguments);
                if (!init.Any(x => x.ContentEquals(findTrue)) && !init.Any(x => x.ContentEquals(fact)))
                    newInits.Add(fact);
            }
            return newInits.ToList();
        }
    }
}
