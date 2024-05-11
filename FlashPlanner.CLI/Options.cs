using CommandLine;

namespace FlashPlanner.CLI
{
    public class Options
    {
        [Option("domain", Required = true, HelpText = "Path to the domain file")]
        public string DomainPath { get; set; } = "";
        [Option("problem", Required = true, HelpText = "Path to the problem file")]
        public string ProblemPath { get; set; } = "";
        [Option("search", Required = true, HelpText = "What search algorithm to use")]
        public string SearchOption { get; set; } = "";
        [Option("translator", Required = false, HelpText = "What translator to use")]
        public string TranslatorOption { get; set; } = "primary(true)";

        [Option("plan", Required = false, HelpText = "Target output plan file")]
        public string PlanPath { get; set; } = "";
        [Option("translator-time-limit", Required = false, HelpText = "Time limit for the translator in seconds. (-1 is no time limit)", Default = -1)]
        public int TranslatorTimeLimit { get; set; } = -1;
        [Option("search-time-limit", Required = false, HelpText = "Time limit for the search in seconds. (-1 is no time limit)", Default = -1)]
        public int SearchTimeLimit { get; set; } = -1;
    }
}
