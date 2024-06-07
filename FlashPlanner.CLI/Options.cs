using CommandLine;

namespace FlashPlanner.CLI
{
    public class Options
    {
        [Option("domain", Required = true, HelpText = "Path to the domain file")]
        public string DomainPath { get; set; } = "";
        [Option("problem", Required = true, HelpText = "Path to the problem file")]
        public string ProblemPath { get; set; } = "";
        [Option("search", Required = false, HelpText = "What search algorithm to use")]
        public string SearchOption { get; set; } = "";
        [Option("translator", Required = false, HelpText = "What translator to use")]
        public string TranslatorOption { get; set; } = "primary(false)";
        [Option("alias", Required = false, HelpText = "An alias that represents a configuration for the search and translation")]
        public string AliasOption { get; set; } = "";

        [Option("print-plan", Required = false, HelpText = "Should the resulting plan be printed to stdout?", Default = false)]
        public bool PrintPlan { get; set; } = false;
        [Option("plan", Required = false, HelpText = "Target output plan file", Default = "solution.plan")]
        public string PlanPath { get; set; } = "solution.plan";
        [Option("translator-time-limit", Required = false, HelpText = "Time limit for the translator in seconds. (-1 is no time limit)", Default = -1)]
        public int TranslatorTimeLimit { get; set; } = -1;
        [Option("search-time-limit", Required = false, HelpText = "Time limit for the search in seconds. (-1 is no time limit)", Default = -1)]
        public int SearchTimeLimit { get; set; } = -1;
        [Option("translator-memory-limit", Required = false, HelpText = "Memory limit for the translator in MB. (-1 is no limit)", Default = -1)]
        public int TranslatorMemoryLimit { get; set; } = -1;
        [Option("search-memory-limit", Required = false, HelpText = "Memory limit for the search in MB. (-1 is no limit)", Default = -1)]
        public int SearchMemoryLimit { get; set; } = -1;
        [Option("validate", Required = false, HelpText = "If the final plan that is found should be validated with PlanVal", Default = false)]
        public bool DoValidate { get; set; } = false;
    }
}
