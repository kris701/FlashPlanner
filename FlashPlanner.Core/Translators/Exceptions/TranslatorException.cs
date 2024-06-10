namespace FlashPlanner.Core.Translators.Exceptions
{
    /// <summary>
    /// General translator exception
    /// </summary>
    public class TranslatorException : Exception
    {
        /// <summary>
        /// Constructor for translator exception
        /// </summary>
        /// <param name="message"></param>
        public TranslatorException(string? message) : base(message)
        {
        }
    }
}
