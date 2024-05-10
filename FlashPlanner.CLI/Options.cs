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

        [Option("plan", Required = false, HelpText = "Target output plan file")]
        public string PlanPath { get; set; } = "";
    }
}
