using FlashPlanner.Core.Models;

namespace FlashPlanner.Core.Translators.Phases
{
    /// <summary>
    /// Represents some phase in the translation.
    /// </summary>
    public interface ITranslatorPhase
    {
        public bool Abort { get; set; }
        public event LogEventHandler? DoLog;
        public TranslatorContext ExecutePhase(TranslatorContext from);
    }
}
