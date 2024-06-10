using FlashPlanner.ArgumentParsing;
using FlashPlanner.Core.Search;
using FlashPlanner.Core.Translators;
using PDDLSharp.Models.PDDL;

namespace FlashPlanner
{
    public static class InputArgumentBuilder
    {
        public static IPlanner GetPlanner(PDDLDecl pddlDecl, string search)
        {
            var dict = new Dictionary<string, object?>();
            dict.Add("pddl", pddlDecl);

            var target = ArgumentBuilder.Parse(search, dict, Registers.SearchRegister);
            if (target is IPlanner planner)
                return planner;
            throw new Exception("Invalid search argument!");
        }

        public static ITranslator GetTranslator(PDDLDecl pddlDecl, string translator)
        {
            var dict = new Dictionary<string, object?>();
            dict.Add("pddl", pddlDecl);

            var target = ArgumentBuilder.Parse(translator, dict, Registers.TranslatorRegister);
            if (target is ITranslator planner)
                return planner;
            throw new Exception("Invalid translator argument!");
        }

        public static AliasArgument GetAlias(string aliasText)
        {
            var target = ArgumentBuilder.Parse(aliasText, new Dictionary<string, object?>(), Registers.AliasRegister);
            if (target is AliasArgument alias)
                return alias;
            throw new Exception("Invalid alias argument!");
        }
    }
}
