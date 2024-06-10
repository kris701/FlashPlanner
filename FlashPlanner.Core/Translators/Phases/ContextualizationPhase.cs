using FlashPlanner.Core.Models;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;

namespace FlashPlanner.Core.Translators.Phases
{
    /// <summary>
    /// Contextualize the <seealso cref="PDDLSharp.Models.PDDL.PDDLDecl"/>
    /// </summary>
    public class ContextualizationPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public ContextualizationPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            if (!from.PDDL.IsContextualised)
            {
                DoLog?.Invoke($"Contextualizing...");
                var listener = new ErrorListener();
                var contextualiser = new PDDLContextualiser(listener);
                contextualiser.Contexturalise(from.PDDL);
            }
            return from;
        }
    }
}
