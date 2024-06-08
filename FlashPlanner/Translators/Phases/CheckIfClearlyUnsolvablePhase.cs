using FlashPlanner.Models;
using FlashPlanner.Models.SAS;

namespace FlashPlanner.Translators.Phases
{
    public class CheckIfClearlyUnsolvablePhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public CheckIfClearlyUnsolvablePhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            foreach (var goal in from.SAS.Goal)
            {
                if (!from.SAS.Operators.Any(x => x.Add.Contains(goal)))
                {
                    from.SAS = new SASDecl(new List<Operator>(), from.SAS.Goal, from.SAS.Init, from.SAS.Facts);
                    DoLog?.Invoke($"Goal fact '{goal}' cannot be reached! Removing all operators!");
                    break;
                }
            }
            return from;
        }
    }
}
