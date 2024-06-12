using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.Translators.Phases
{
    /// <summary>
    /// Generate an applicability graph for the <seealso cref="SASDecl"/>s <seealso cref="Operator"/>s.
    /// If all the operators are bound by the same argument (e.g. in Rovers all the actions are bound by the 'rover0' argument), then ignore the graph and just make a total graph.
    /// Otherwise, connect operators that has at least some arguments or add -> pre combinations in common.
    /// Also set all operators to contain the applicable operators from the initial state. (so we can "return" to the start and continue)
    /// </summary>
    public class GenerateApplicabilityGraphPhase : BaseTranslatorPhase
    {
        public override event LogEventHandler? DoLog;
        public GenerateApplicabilityGraphPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Generate applicability graphs of operators...");
            var argGraph = GenerateOpArgumentGraph(from.SAS);
            if (argGraph.Any(x => x.Value.Count == from.SAS.Operators.Count))
            {
                DoLog?.Invoke($"Fully connected operators detected! Generating total graph instead...");
                return GenerateTotalGraph(from);
            }
            var preGraph = GeneratePreconditionGraph(from.SAS);
            return GenerateApplicabilityGraph(from, argGraph, preGraph);
        }

        private TranslatorContext GenerateTotalGraph(TranslatorContext from)
        {
            var graphs = new LinkedGraph(from.SAS.Operators.Count + 1);
            var inits = GetInitApplicableOperators(from);
            var all = from.SAS.Operators.Select(x => x.ID).ToList();
            // 0 is from the initial state
            graphs.LinkAll(0, inits);
            foreach (var op in from.SAS.Operators)
                graphs.LinkAll(op.ID, all);

            from = new TranslatorContext(from) { ApplicabilityGraph = graphs };
            return from;
        }

        private TranslatorContext GenerateApplicabilityGraph(TranslatorContext from, Dictionary<string, List<int>> argGraph, Dictionary<int, List<int>> preGraph)
        {
            var graphs = new LinkedGraph(from.SAS.Operators.Count + 1);
            var inits = GetInitApplicableOperators(from);
            // 0 is from the initial state
            graphs.LinkAll(0, inits);
            foreach (var op in from.SAS.Operators)
            {
                var possibles = new List<int>();
                foreach (var arg in op.Arguments)
                    if (argGraph.TryGetValue(arg, out List<int>? value))
                        possibles.AddRange(value);
                foreach (var add in op.Add)
                    if (preGraph.TryGetValue(add.ID, out List<int>? value))
                        possibles.AddRange(value);
                possibles.AddRange(inits);
                graphs.LinkAll(op.ID, possibles);
            }

            var total = graphs.Count;
            var worst = from.SAS.Operators.Count * from.SAS.Operators.Count;
            DoLog?.Invoke($"Applicability graph reduces operator checking to {Math.Round((double)total / worst * 100, 2)}% of max");

            from = new TranslatorContext(from) { ApplicabilityGraph = graphs };
            return from;
        }

        private List<int> GetInitApplicableOperators(TranslatorContext context)
        {
            var ops = new List<int>();
            var initState = new SASStateSpace(context);

            foreach (var op in context.SAS.Operators)
                if (initState.IsApplicable(op))
                    ops.Add(op.ID);

            return ops;
        }

        private Dictionary<string, List<int>> GenerateOpArgumentGraph(SASDecl decl)
        {
            var argGraph = new Dictionary<string, List<int>>();
            foreach (var op in decl.Operators)
            {
                foreach (var arg in op.Arguments)
                {
                    if (argGraph.ContainsKey(arg))
                        argGraph[arg].Add(op.ID);
                    else
                        argGraph.Add(arg, new List<int>() { op.ID });
                }
            }
            foreach (var key in argGraph.Keys)
                argGraph[key] = argGraph[key].Distinct().ToList();
            return argGraph;
        }

        private Dictionary<int, List<int>> GeneratePreconditionGraph(SASDecl decl)
        {
            var preGraph = new Dictionary<int, List<int>>();
            foreach (var op in decl.Operators)
            {
                foreach (var pre in op.Pre)
                {
                    if (preGraph.ContainsKey(pre.ID))
                        preGraph[pre.ID].Add(op.ID);
                    else
                        preGraph.Add(pre.ID, new List<int>() { op.ID });
                }
            }
            foreach (var key in preGraph.Keys)
                preGraph[key] = preGraph[key].Distinct().ToList();
            return preGraph;
        }
    }
}
