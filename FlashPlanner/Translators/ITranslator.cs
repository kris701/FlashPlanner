using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Translators
{
    /// <summary>
    /// Main interface to convert a <seealso cref="PDDLDecl"/> into a <seealso cref="SASDecl"/>
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Time it took to translate
        /// </summary>
        public TimeSpan TranslationTime { get; }
        /// <summary>
        /// Time limit for the translation
        /// </summary>
        public TimeSpan TimeLimit { get; set; }
        /// <summary>
        /// Aborted is true if the time limit was reached
        /// </summary>
        public bool Aborted { get; }

        /// <summary>
        /// How many facts have been created during the translation
        /// </summary>
        public int Facts { get; }
        /// <summary>
        /// How many operators have been created during the translation
        /// </summary>
        public int Operators { get; }

        /// <summary>
        /// Convert a <seealso cref="PDDLDecl"/> into a <seealso cref="SASDecl"/>
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public SASDecl Translate(PDDLDecl from);
    }
}
