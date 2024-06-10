using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;

namespace FlashPlanner.Core.Translators.Phases
{
    public class ResetOperatorsPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public ResetOperatorsPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Resetting operators...");
            var resetOps = new List<Operator>();
            foreach (var op in from.SAS.Operators)
            {
                var reset = new Operator(op.Name, op.Arguments, op.Pre, op.Add, op.Del, from.SAS.Facts);
                reset.ID = op.ID;
                resetOps.Add(reset);
            }
            from.SAS = new SASDecl(resetOps, from.SAS.Goal, from.SAS.Init, from.SAS.Facts);
            return from;
        }
    }
}
