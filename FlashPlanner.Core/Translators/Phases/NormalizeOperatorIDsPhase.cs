using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;

namespace FlashPlanner.Core.Translators.Phases
{
    /// <summary>
    /// Take all <seealso cref="Operator"/>s that exist, and give them IDs from 0 and up.
    /// This is needed, since the ID is what distinguishes unique operators.
    /// </summary>
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
