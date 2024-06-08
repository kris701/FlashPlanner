using FlashPlanner.Models;
using FlashPlanner.Models.SAS;
using FlashPlanner.States;
using PDDLSharp.Tools;

namespace FlashPlanner.Translators.Phases
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
            var graphs = new Dictionary<int, List<Operator>>();
            var inits = GetInitApplicableOperators(from);
            // -1 is from the initial state
            graphs.Add(-1, inits.ToList());
            var preGraph = GeneratePreconditionGraph(from.SAS);

            foreach (var op in from.SAS.Operators)
            {
                var possibles = new HashSet<Operator>();
                foreach (var add in op.Add)
                    foreach (var arg in add.Arguments)
                        if (preGraph.ContainsKey(arg))
                            possibles.AddRange(preGraph[arg]);
                foreach (var pre in op.Pre)
                    foreach (var arg in pre.Arguments)
                        if (preGraph.ContainsKey(arg))
                            possibles.AddRange(preGraph[arg]);
                possibles.AddRange(inits);
                graphs.Add(op.ID, possibles.ToList());
            }

            foreach (var key in graphs.Keys)
                graphs[key].RemoveAll(x => x.ID == key);

            var total = graphs.Sum(x => x.Value.Count);
            var worst = from.SAS.Operators.Count * from.SAS.Operators.Count;
            DoLog?.Invoke($"Applicability graph reduces operator checking to {Math.Round(((double)total / worst) * 100, 2)}% of max");

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

        private Dictionary<string, HashSet<Operator>> GeneratePreconditionGraph(SASDecl decl)
        {
            var preGraph = new Dictionary<string, HashSet<Operator>>();
            foreach (var op in decl.Operators)
            {
                foreach (var pre in op.Pre)
                {
                    foreach (var arg in pre.Arguments)
                    {
                        if (preGraph.ContainsKey(arg))
                            preGraph[arg].Add(op);
                        else
                            preGraph.Add(arg, new HashSet<Operator>() { op });
                    }
                }
            }
            return preGraph;
        }
    }
}
