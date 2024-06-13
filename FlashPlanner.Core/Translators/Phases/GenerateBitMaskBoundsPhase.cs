using FlashPlanner.Core.Models;

namespace FlashPlanner.Core.Translators.Phases
{
    public class GenerateBitMaskBoundsPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public GenerateBitMaskBoundsPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Setting bounds for bitmasks...");
            from.ApplicabilityGraph.GenerateBounds();
            from.SAS.GoalMask.GenerateBounds();
            return from;
        }
    }
}
