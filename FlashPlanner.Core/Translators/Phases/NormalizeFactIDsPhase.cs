using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;

namespace FlashPlanner.Core.Translators.Phases
{
    /// <summary>
    /// Take all <seealso cref="Fact"/>s that exists from the goal state, init state and operators, and  make sure that equivalent facts have the same ID.
    /// This ID is increasing from 0, so we can make <seealso href="https://en.wikipedia.org/wiki/Bit_array">Bit Arrays</seealso> of it later.
    /// </summary>
    public class NormalizeFactIDsPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public NormalizeFactIDsPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Normalizing remaining fact IDs...");
            RecountFacts(from.SAS);
            from.SAS = new SASDecl(from.SAS.Operators, from.SAS.Goal, from.SAS.Init, from.SAS.Facts);
            return from;
        }

        private void RecountFacts(SASDecl decl)
        {
            var check = new List<Fact>();
            check.AddRange(decl.Init);
            check.AddRange(decl.Goal);
            foreach (var op in decl.Operators)
            {
                check.AddRange(op.Pre);
                check.AddRange(op.Add);
                check.AddRange(op.Del);
            }
            var unique = new List<Fact>();
            while (check.Count > 0)
            {
                var fact = check[0];
                unique.Add(fact);
                check.RemoveAll(x => x.ContentEquals(fact));
            }
            var uniqueDict = new Dictionary<string, Fact>();
            foreach (var fact in unique)
                uniqueDict.Add(fact.ToString(), fact);
            int count = 0;
            foreach (var fact in unique)
                fact.ID = count++;

            ReplaceFacts(decl.Init, uniqueDict);
            ReplaceFacts(decl.Goal, uniqueDict);
            foreach (var op in decl.Operators)
            {
                ReplaceFacts(op.Pre, uniqueDict);
                ReplaceFacts(op.Add, uniqueDict);
                ReplaceFacts(op.Del, uniqueDict);
            }

            decl.Facts = count;
        }

        private void ReplaceFacts(Fact[] from, Dictionary<string, Fact> with)
        {
            for (int i = 0; i < from.Length; i++)
                from[i] = with[from[i].ToString()];
        }
    }
}
