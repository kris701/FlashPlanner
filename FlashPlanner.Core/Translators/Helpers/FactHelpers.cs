using FlashPlanner.Core.Models.SAS;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;

namespace FlashPlanner.Core.Translators.Helpers
{
    public static class FactHelpers
    {
        public static readonly string NegatedPrefix = "$neg-";

        public static Fact GetFactFromPredicate(PredicateExp pred)
        {
            var name = pred.Name;
            var args = new List<string>();
            foreach (var arg in pred.Arguments)
                args.Add(arg.Name);
            var newFact = new Fact(name, args.ToArray());
            return newFact;
        }

        public static Fact GetNegatedOf(Fact fact) => new Fact($"{NegatedPrefix}{fact.Name}", fact.Arguments);

        public static Dictionary<bool, List<Fact>> ExtractFactsFromExp(IExp exp, bool possitive = true)
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

        private static Dictionary<bool, List<Fact>> MergeDictionaries(Dictionary<bool, List<Fact>> dict1, Dictionary<bool, List<Fact>> dict2)
        {
            var resultDict = new Dictionary<bool, List<Fact>>();
            foreach (var key in dict1.Keys)
                resultDict.Add(key, dict1[key]);
            foreach (var key in dict2.Keys)
                resultDict[key].AddRange(dict2[key]);

            return resultDict;
        }
    }
}
