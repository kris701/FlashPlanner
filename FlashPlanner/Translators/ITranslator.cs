using FlashPlanner.Models;
using PDDLSharp.Models.PDDL;

namespace FlashPlanner.Translators
{
    /// <summary>
    /// Main interface to convert a <seealso cref="PDDLDecl"/> into a <seealso cref="TranslatorContext"/>
    /// </summary>
    public interface ITranslator : ILimitedComponent
    {
        /// <summary>
        /// Convert a <seealso cref="PDDLDecl"/> into a <seealso cref="TranslatorContext"/>
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public TranslatorContext Translate(PDDLDecl from);
    }
}
