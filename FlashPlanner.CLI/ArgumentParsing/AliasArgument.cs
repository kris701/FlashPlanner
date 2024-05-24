using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.CLI.ArgumentParsing
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
