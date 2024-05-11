using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Translators
{
    public interface ITranslator
    {
        public TimeSpan TranslationTime { get; }
        public TimeSpan TimeLimit { get; set; }
        public bool Aborted { get; }

        public int Facts { get; }
        public int Operators { get; }

        public SASDecl Translate(PDDLDecl from);
    }
}
