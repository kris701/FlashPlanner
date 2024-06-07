using FlashPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Translators.Phases
{
    public abstract class BaseTranslatorPhase : ITranslatorPhase
    {
        public bool Abort { get; set; }
        public abstract event LogEventHandler? DoLog;

        public abstract TranslatorContext ExecutePhase(TranslatorContext from);
    }
}
