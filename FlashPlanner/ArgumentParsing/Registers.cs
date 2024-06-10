using FlashPlanner.Core.Heuristics;

namespace FlashPlanner.ArgumentParsing
{
    public static class Registers
    {
        public static List<Argument> AliasRegister = new List<Argument>()
        {
            new Argument("fast", new Dictionary<string, Type>(), (args) =>
            {
                return new AliasArgument(
                    "greedy(hFF())",
                    "primary(false)");
            }),
        };

        public static List<Argument> TranslatorRegister = new List<Argument>()
        {
            new Argument("primary", new Dictionary<string, Type>()
            {
                { "noEqualsParams", typeof(bool) },
            }, (args) =>
            {
                if (args["noEqualsParams"] is bool noEquals)
                    return new PDDLToSASTranslator(noEquals);
                throw new Exception("Invalid arguments given for translator!");
            })
        };

        public static List<Argument> SearchRegister = new List<Argument>()
        {
            // Search (Classical)
            new Argument("greedy", new Dictionary<string, Type>(){
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["h"] is IHeuristic h)
                    return new GreedyBFS(h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new Argument("greedy_defered", new Dictionary<string, Type>(){
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["h"] is IHeuristic h)
                    return new GreedyBFSDHE(h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new Argument("greedy_prefered", new Dictionary<string, Type>(){
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["h"] is IHeuristic h)
                    return new GreedyBFSPO(h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new Argument("greedy_underaprox", new Dictionary<string, Type>(){
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["h"] is IHeuristic h)
                    return new GreedyBFSUAR(h);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new Argument("beam", new Dictionary<string, Type>(){
                { "h", typeof(IHeuristic) },
                { "b", typeof(int) }
            }, (args) =>
            {
                if (args["h"] is IHeuristic h && args["b"] is int beta)
                    return new BeamS(h, beta);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new Argument("greedy_focused", new Dictionary<string, Type>(){
                { "h", typeof(IHeuristic) },
                { "n", typeof(int) },
                { "b", typeof(int) },
                { "p", typeof(int) },
            }, (args) =>
            {
                if (args["h"] is IHeuristic h &&
                    args["n"] is int n &&
                    args["b"] is int b &&
                    args["p"] is int p)
                    return new GreedyBFSFocused(h, n, b, p);
                throw new Exception("Invalid arguments given for planner!");
            }),
            new Argument("greedy_lazy", new Dictionary<string, Type>(){
                { "h", typeof(IHeuristic) }
            }, (args) =>
            {
                if (args["h"] is IHeuristic h)
                    return new GreedyBFSLazy(h);
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
            new Argument("hFF", new Dictionary<string, Type>(), (args) => new hFF()),
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
