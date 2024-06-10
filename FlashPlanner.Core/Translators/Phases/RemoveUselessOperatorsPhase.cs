using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;

namespace FlashPlanner.Core.Translators.Phases
{
    public class RemoveUselessOperatorsPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public RemoveUselessOperatorsPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Removing operators that are useless...");
            var newOps = new List<Operator>();
            foreach (var op in from.SAS.Operators)
            {
                if (op.Add.All(x => op.Del.Any(y => y.ContentEquals(x))))
                    continue;
                if (op.Add.All(x => op.Pre.Any(y => y.ContentEquals(x))))
                    continue;

                newOps.Add(op);
            }
            DoLog?.Invoke($"Removed {from.SAS.Operators.Count - newOps.Count} useless operators.");
            from.SAS = new SASDecl(newOps, from.SAS.Goal, from.SAS.Init, from.SAS.Facts);
            return from;
        }
    }
}
