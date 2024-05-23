using FlashPlanner.Heuristics;
using FlashPlanner.HeuristicsCollections;
using FlashPlanner.Translators;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.CLI.ArgumentParsing
{
    public static class Registers
    {
        public static List<Argument> TranslatorRegister = new List<Argument>()
        {
            new Argument("primary", new Dictionary<string, Type>()
            {
                { "removeStatics", typeof(bool) },
                { "noEqualsParams", typeof(bool) },
            }, (args) =>
            {
                if (args["removeStatics"] is bool remove && args["noEqualsParams"] is bool noEquals)
                    return new PDDLToSASTranslator(remove, noEquals);
                throw new Exception("Invalid arguments given for translator!");
            })
        };

        public static List<Argument> SearchRegister = new List<Argument>()
        {
            // Search (Classical)
            new Argument("greedy", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["h"] is IHeuristic h)
                    return new Search.Classical.GreedyBFS(sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new Argument("greedy_defered", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["h"] is IHeuristic h)
                    return new Search.Classical.GreedyBFSDHE(sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new Argument("greedy_prefered", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["h"] is IHeuristic h)
                    return new Search.Classical.GreedyBFSPO(sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new Argument("greedy_underaprox", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["h"] is IHeuristic h)
                    return new Search.Classical.GreedyBFSUAR(sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new Argument("beam", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) },
                { "b", typeof(int) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["h"] is IHeuristic h && args["b"] is int beta)
                    return new Search.Classical.BeamS(sas, h, beta);
                throw new Exception("Invalid arguments given for planner!");
            }),

            // Search (BlackBox)
            new Argument("greedy_bb", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["h"] is IHeuristic h)
                    return new Search.BlackBox.GreedyBFS(sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new Argument("greedy_bb_focused", new Dictionary<string, Type>(){
                { "sas", typeof(SASDecl) },
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["sas"] is SASDecl sas && args["pddl"] is PDDLDecl pddl && args["h"] is IHeuristic h)
                    return new Search.BlackBox.GreedyBFSFocused(pddl, sas, h);
                throw new Exception("Invalid arguments given for planner!");
            }),

            // Heuristics
            new Argument("hAdd", new Dictionary<string, Type>(), (args) => new hAdd()),
            new Argument("hConstant", new Dictionary<string, Type>(){
                { "c", typeof(int) }
            }, (args) =>
            {
                if (args["c"] is int constant)
                    return new hConstant(constant);
                throw new Exception("Invalid arguments given for heuristic!");
            }),
            new Argument("hDepth", new Dictionary<string, Type>(), (args) => new hDepth()),
            new Argument("hFF", new Dictionary<string, Type>(), (args) =>
            {
                if (args["sas"] is SASDecl sas)
                    return new hFF(sas);
                throw new Exception("Invalid arguments given for heuristic!");
            }),
            new Argument("hGoal", new Dictionary<string, Type>(), (args) => new hGoal()),
            new Argument("hMax", new Dictionary<string, Type>(), (args) => new hMax()),
            new Argument("hPath", new Dictionary<string, Type>(), (args) => new hPath()),
            new Argument("hWeighted", new Dictionary<string, Type>(){
                { "hw", typeof(IHeuristic) },
                { "w", typeof(double) }
            }, (args) =>
            {
                if (args["hw"] is IHeuristic h && args["w"] is double w)
                    return new hWeighted(h, w);
                throw new Exception("Invalid arguments given for heuristic!");
            }),

            // Collection Heuristics
            new Argument("hColSum", new Dictionary<string, Type>()
            {
                { "lists", typeof(List<IHeuristic>) }
            }, (args) =>
            {
                if (args["lists"] is List<IHeuristic> listH)
                    return new hColSum(listH);
                throw new Exception("Invalid arguments given for heuristic!");
            }),
            new Argument("hColMax", new Dictionary<string, Type>()
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
