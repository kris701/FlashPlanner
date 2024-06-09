﻿using FlashPlanner.Models;
using FlashPlanner.Models.SAS;

namespace FlashPlanner.Translators.Phases
{
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
            foreach(var fact in check)
                if (!unique.Any(x => x.ContentEquals(fact)))
                    unique.Add(fact);
            int count = 0;
            foreach (var fact in unique)
                fact.ID = count++;

            ReplaceFacts(decl.Init, unique);
            ReplaceFacts(decl.Goal, unique);
            foreach (var op in decl.Operators)
            {
                ReplaceFacts(op.Pre, unique);
                ReplaceFacts(op.Add, unique);
                ReplaceFacts(op.Del, unique);
            }

            decl.Facts = count;
        }

        private void ReplaceFacts(Fact[] from, List<Fact> with)
        {
            for (int i = 0; i < from.Length; i++)
                from[i] = with.First(x => x.ContentEquals(from[i]));
        }
    }
}
