using FlashPlanner.Core.Models;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
