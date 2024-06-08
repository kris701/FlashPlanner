using FlashPlanner.Models;

namespace FlashPlanner.Translators.Phases
{
    public interface ITranslatorPhase
    {
        public bool Abort { get; set; }
        public event LogEventHandler? DoLog;
        public TranslatorContext ExecutePhase(TranslatorContext from);
    }
}
