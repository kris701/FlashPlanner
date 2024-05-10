using FlashPlanner.Heuristics;
using FlashPlanner.HeuristicsCollections;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.CLI.SearchParsing
{
    public static class SearchConstructors
    {
        public static List<SearchOption> Register = new List<SearchOption>()
        {
            // Search (Classical)
            new SearchOption("greedy", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["h"] is IHeuristic h)
                    return new Search.Classical.GreedyBFS(sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new SearchOption("greedy_defered", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["h"] is IHeuristic h)
                    return new Search.Classical.GreedyBFSDHE(sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new SearchOption("greedy_prefered", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["h"] is IHeuristic h)
                    return new Search.Classical.GreedyBFSPO(sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new SearchOption("greedy_underaprox", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["h"] is IHeuristic h)
                    return new Search.Classical.GreedyBFSUAR(sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),

            // Search (BlackBox)
            new SearchOption("greedy_bb", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["h"] is IHeuristic h)
                    return new Search.BlackBox.GreedyBFS(sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new SearchOption("greedy_bb_focused", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["pddl"] is PDDLDecl pddl && args["h"] is IHeuristic h)
                    return new Search.BlackBox.GreedyBFSFocused(pddl, sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),

            // Heuristics
            new SearchOption("hAdd", new Dictionary<string, Type>(), (args) => new hAdd()),
            new SearchOption("hConstant", new Dictionary<string, Type>(){
                { "c", typeof(int) }
            }, (args) =>
            {
                if (args["c"] is int constant)
                    return new hConstant(constant);
                throw new Exception("Invalid arguments given for heuristic!");
            }),
            new SearchOption("hDepth", new Dictionary<string, Type>(), (args) => new hDepth()),
            new SearchOption("hFF", new Dictionary<string, Type>(), (args) =>
            {
                if (args["sas"] is SASDecl sas)
                    return new hFF(sas);
                throw new Exception("Invalid arguments given for heuristic!");
            }),
            new SearchOption("hGoal", new Dictionary<string, Type>(), (args) => new hGoal()),
            new SearchOption("hMax", new Dictionary<string, Type>(), (args) => new hMax()),
            new SearchOption("hPath", new Dictionary<string, Type>(), (args) => new hPath()),
            new SearchOption("hWeighted", new Dictionary<string, Type>(){
                { "hw", typeof(IHeuristic) },
                { "w", typeof(double) }
            }, (args) =>
            {
                if (args["hw"] is IHeuristic h && args["w"] is double w)
                    return new hWeighted(h, w);
                throw new Exception("Invalid arguments given for heuristic!");
            }),

            // Collection Heuristics
            new SearchOption("hColSum", new Dictionary<string, Type>()
            {
                { "lists", typeof(List<IHeuristic>) }
            }, (args) =>
            {
                if (args["lists"] is List<IHeuristic> listH)
                    return new hColSum(listH);
                throw new Exception("Invalid arguments given for heuristic!");
            }),
            new SearchOption("hColMax", new Dictionary<string, Type>()
            {
                { "listm", typeof(List<IHeuristic>) }
            }, (args) =>
            {
                if (args["listm"] is List<IHeuristic> listH)
                    return new hColMax(listH);
                throw new Exception("Invalid arguments given for heuristic!");
            }),
        };
    }
}
