using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;

namespace FlashPlanner.Core.Translators.Phases
{
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
            var graphs = new Dictionary<int, List<Operator>>();
            var inits = GetInitApplicableOperators(from);
            // -1 is from the initial state
            graphs.Add(-1, inits.ToList());
            foreach (var op in from.SAS.Operators)
                graphs.Add(op.ID, from.SAS.Operators);

            from = new TranslatorContext(from.SAS, from.PDDL, from.FactHashes, graphs);
            return from;
        }

        private TranslatorContext GenerateApplicabilityGraph(TranslatorContext from, Dictionary<string, List<Operator>> argGraph, Dictionary<int, List<Operator>> preGraph)
        {
            var graphs = new Dictionary<int, List<Operator>>();
            var inits = GetInitApplicableOperators(from);
            // -1 is from the initial state
            graphs.Add(-1, inits.ToList());
            foreach (var op in from.SAS.Operators)
            {
                var possibles = new List<Operator>();
                foreach (var arg in op.Arguments)
                    if (argGraph.ContainsKey(arg))
                        possibles.AddRange(argGraph[arg]);
                foreach (var add in op.Add)
                    if (preGraph.ContainsKey(add.ID))
                        possibles.AddRange(preGraph[add.ID]);
                possibles.AddRange(inits);
                graphs.Add(op.ID, possibles.DistinctBy(x => x.ID).ToList());
            }

            foreach (var key in graphs.Keys)
                graphs[key].RemoveAll(x => x.ID == key);

            var total = graphs.Sum(x => x.Value.Count);
            var worst = from.SAS.Operators.Count * from.SAS.Operators.Count;
            DoLog?.Invoke($"Applicability graph reduces operator checking to {Math.Round((double)total / worst * 100, 2)}% of max");

            from = new TranslatorContext(from.SAS, from.PDDL, from.FactHashes, graphs);
            return from;
        }

        private HashSet<Operator> GetInitApplicableOperators(TranslatorContext context)
        {
            var ops = new HashSet<Operator>();
            var initState = new SASStateSpace(context);

            foreach (var op in context.SAS.Operators)
                if (initState.IsApplicable(op))
                    ops.Add(op);

            return ops;
        }

        private Dictionary<string, List<Operator>> GenerateOpArgumentGraph(SASDecl decl)
        {
            var argGraph = new Dictionary<string, List<Operator>>();
            foreach (var op in decl.Operators)
            {
                foreach (var arg in op.Arguments)
                {
                    if (argGraph.ContainsKey(arg))
                        argGraph[arg].Add(op);
                    else
                        argGraph.Add(arg, new List<Operator>() { op });
                }
            }
            foreach (var key in argGraph.Keys)
                argGraph[key] = argGraph[key].Distinct().ToList();
            return argGraph;
        }

        private Dictionary<int, List<Operator>> GeneratePreconditionGraph(SASDecl decl)
        {
            var preGraph = new Dictionary<int, List<Operator>>();
            foreach (var op in decl.Operators)
            {
                foreach (var pre in op.Pre)
                {
                    if (preGraph.ContainsKey(pre.ID))
                        preGraph[pre.ID].Add(op);
                    else
                        preGraph.Add(pre.ID, new List<Operator>() { op });
                }
            }
            foreach (var key in preGraph.Keys)
                preGraph[key] = preGraph[key].Distinct().ToList();
            return preGraph;
        }
    }
}
