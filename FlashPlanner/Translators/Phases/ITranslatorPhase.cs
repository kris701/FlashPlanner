using FlashPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Translators.Phases
{
    public interface ITranslatorPhase
    {
        public bool Abort { get; set; }
        public event LogEventHandler? DoLog;
        public TranslatorContext ExecutePhase(TranslatorContext from);
    }
}
