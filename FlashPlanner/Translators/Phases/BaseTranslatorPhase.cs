using FlashPlanner.Models;

namespace FlashPlanner.Translators.Phases
{
    public abstract class BaseTranslatorPhase : ITranslatorPhase
    {
        public bool Abort { get; set; }
        public abstract event LogEventHandler? DoLog;

        public abstract TranslatorContext ExecutePhase(TranslatorContext from);
    }
}
