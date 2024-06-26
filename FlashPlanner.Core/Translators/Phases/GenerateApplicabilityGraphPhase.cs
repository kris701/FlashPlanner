﻿using FlashPlanner.Core.Models;
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
        private static readonly uint _initialStateID = 0;
        public GenerateApplicabilityGraphPhase(LogEventHandler? doLog)
        {
            DoLog = doLog;
        }

        public override TranslatorContext ExecutePhase(TranslatorContext from)
        {
            DoLog?.Invoke($"Generate applicability graphs of operators...");
            var argGraph = GenerateOpArgumentGraph(from.SAS);
            if (argGraph.Any(x => x.Value.GetTrueBits() == from.SAS.Operators.Count))
            {
                DoLog?.Invoke($"Fully connected operators detected! Generating total graph instead...");
                return GenerateTotalGraph(from);
            }
            var preGraph = GeneratePreconditionGraph(from.SAS);
            return GenerateApplicabilityGraph(from, argGraph, preGraph);
        }

        private TranslatorContext GenerateTotalGraph(TranslatorContext from)
        {
            var baseMask = new BitMask((uint)from.SAS.Operators.Count + 1);
            for (uint i = 1; i < from.SAS.Operators.Count; i++)
                baseMask[i] = true;
            var matrixes = new BitMask[(uint)from.SAS.Operators.Count + 1];
            foreach (var op in from.SAS.Operators)
                matrixes[op.ID] = baseMask;
            matrixes[0] = new BitMask((uint)from.SAS.Operators.Count + 1);

            var graphs = new LinkedGraph(matrixes);
            var inits = GetInitApplicableOperators(from);
            graphs.LinkAll(_initialStateID, inits);

            from = new TranslatorContext(from) { ApplicabilityGraph = graphs };
            return from;
        }

        private TranslatorContext GenerateApplicabilityGraph(TranslatorContext from, Dictionary<string, BitMask> argGraph, Dictionary<uint, BitMask> preGraph)
        {
            var matrixes = new BitMask[(uint)from.SAS.Operators.Count + 1];
            var inits = GetInitApplicableOperators(from);
            var baseMask = new BitMask((uint)from.SAS.Operators.Count + 1);
            foreach (var id in inits)
                baseMask[id] = true;
            matrixes[0] = baseMask;

            foreach (var op in from.SAS.Operators)
            {
                var newMask = new BitMask(baseMask);
                foreach (var arg in op.Arguments)
                    newMask.Or(argGraph[arg]);
                foreach (var add in op.Add)
                    if (preGraph.ContainsKey(add.ID))
                        newMask.Or(preGraph[add.ID]);
                matrixes[op.ID] = newMask;
            }

            var graphs = new LinkedGraph(matrixes);

            DoLog?.Invoke($"Applicability graph generated!");
            //float total = graphs.Count;
            //float worst = (float)from.SAS.Operators.Count * (float)from.SAS.Operators.Count;
            //DoLog?.Invoke($"Applicability graph reduces operator checking to {Math.Round((double)total / worst * 100, 2)}% of max");

            from = new TranslatorContext(from) { ApplicabilityGraph = graphs };
            return from;
        }

        private List<uint> GetInitApplicableOperators(TranslatorContext context)
        {
            var ops = new List<uint>();
            var initState = new SASStateSpace(context);

            foreach (var op in context.SAS.Operators)
                if (initState.IsApplicable(op))
                    ops.Add(op.ID);

            return ops;
        }

        private Dictionary<string, BitMask> GenerateOpArgumentGraph(SASDecl decl)
        {
            var argRefs = new Dictionary<string, List<uint>>();
            foreach (var op in decl.Operators)
            {
                foreach (var arg in op.Arguments)
                {
                    if (argRefs.ContainsKey(arg))
                        argRefs[arg].Add(op.ID);
                    else
                        argRefs.Add(arg, new List<uint>() { op.ID });
                }
            }

            var argGraph = new Dictionary<string, BitMask>();
            foreach (var arg in argRefs.Keys)
            {
                var newMask = new BitMask((uint)decl.Operators.Count + 1);
                foreach (var id in argRefs[arg])
                    newMask[id] = true;
                argGraph.Add(arg, newMask);
            }

            return argGraph;
        }

        private Dictionary<uint, BitMask> GeneratePreconditionGraph(SASDecl decl)
        {
            var preRef = new Dictionary<uint, List<uint>>();
            foreach (var op in decl.Operators)
            {
                foreach (var pre in op.Pre)
                {
                    if (preRef.ContainsKey(pre.ID))
                        preRef[pre.ID].Add(op.ID);
                    else
                        preRef.Add(pre.ID, new List<uint>() { op.ID });
                }
            }
            var preGraph = new Dictionary<uint, BitMask>();
            foreach (var arg in preRef.Keys)
            {
                var newMask = new BitMask((uint)decl.Operators.Count + 1);
                foreach (var id in preRef[arg])
                    newMask[id] = true;
                preGraph.Add(arg, newMask);
            }

            return preGraph;
        }
    }
}
