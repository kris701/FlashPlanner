using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;

namespace FlashPlanner.Core.Translators.Phases
{
    /// <summary>
    /// Checks if for each goal <seealso cref="Fact"/>, there exists at least one <seealso cref="Operator"/> that has that effect.
    /// If no operator have said effect, remove all operators and assume the problem to be unsolvable.
    /// </summary>
    public class CheckIfClearlyUnsolvablePhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public CheckIfClearlyUnsolvablePhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Checking if task is clearly unsolvable...");
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
