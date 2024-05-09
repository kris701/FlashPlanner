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
        public SearchOptions SearchOption { get; set; }
        [Option("heuristic", Required = true, HelpText = "What heuristic to use for the search")]
        public string HeuristicOption { get; set; }

        [Option("plan", Required = false, HelpText = "Target output plan file")]
        public string PlanPath { get; set; } = "";
    }
}
