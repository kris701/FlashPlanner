using FlashPlanner.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Translators
{
    /// <summary>
    /// Main interface to convert a <seealso cref="PDDLDecl"/> into a <seealso cref="SASDecl"/>
    /// </summary>
    public interface ITranslator : ILimitedComponent
    {
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
        public TranslatorContext Translate(PDDLDecl from);
    }
}
