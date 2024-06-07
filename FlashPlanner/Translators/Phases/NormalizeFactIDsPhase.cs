using FlashPlanner.Models;
using FlashPlanner.Models.SAS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int count = 0;
            var check = new List<Fact>();
            check.AddRange(decl.Init);
            check.AddRange(decl.Goal);
            foreach (var op in decl.Operators)
            {
                check.AddRange(op.Pre);
                check.AddRange(op.Add);
                check.AddRange(op.Del);
            }
            foreach (var fact in check)
                fact.ID = -1;
            for (int i = 0; i < check.Count; i++)
            {
                if (check[i].ID != -1)
                    continue;
                check[i].ID = count;
                for(int j = 0; j < check.Count; j++)
                {
                    if (check[j].ContentEquals(check[i]))
                        check[j].ID = check[i].ID;
                }
                count++;
            }
            decl.Facts = count;
        }
    }
}
