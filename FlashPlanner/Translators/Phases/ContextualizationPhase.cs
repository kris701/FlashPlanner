﻿using FlashPlanner.Models;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Translators.Phases
{
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
