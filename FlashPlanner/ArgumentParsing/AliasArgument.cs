namespace FlashPlanner.ArgumentParsing
{
    public class AliasArgument
    {
        public string SearchOption { get; set; }
        public string TranslatorOption { get; set; }

        public AliasArgument(string searchOption, string translatorOption)
        {
            SearchOption = searchOption;
            TranslatorOption = translatorOption;
        }
    }
}
