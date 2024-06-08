using FlashPlanner.Models;
using FlashPlanner.Models.SAS;

namespace FlashPlanner.Translators.Phases
{
    public class NormalizeOperatorIDsPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public NormalizeOperatorIDsPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Normalizing remaining operator IDs...");
            int count = 0;
            foreach (var op in from.SAS.Operators)
                op.ID = count++;
            from.SAS = new SASDecl(from.SAS.Operators, from.SAS.Goal, from.SAS.Init, from.SAS.Facts);
            return from;
        }
    }
}
