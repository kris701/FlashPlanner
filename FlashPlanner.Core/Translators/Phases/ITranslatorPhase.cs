using FlashPlanner.Core.Models;

namespace FlashPlanner.Core.Translators.Phases
{
    public interface ITranslatorPhase
    {
        public bool Abort { get; set; }
        public event LogEventHandler? DoLog;
        public TranslatorContext ExecutePhase(TranslatorContext from);
    }
}
