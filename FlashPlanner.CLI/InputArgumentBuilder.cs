using FlashPlanner.CLI.ArgumentParsing;
using FlashPlanner.Search;
using FlashPlanner.Translators;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.CLI
{
    public static class InputArgumentBuilder
    {
        public static IPlanner GetPlanner(PDDLDecl pddlDecl, SASDecl sasDecl, string search)
        {
            var dict = new Dictionary<string, object?>();
            dict.Add("sas", sasDecl);
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
    }
}
