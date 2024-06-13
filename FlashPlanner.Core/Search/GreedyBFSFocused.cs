using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.HeuristicsCollections;
using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;
using FlashPlanner.Core.Translators;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkits;

namespace FlashPlanner.Core.Search
{
    /// <summary>
    /// Greedy Best First Search with Focused Macros.
    /// (<seealso href="https://arxiv.org/abs/2004.13242">Efficient Black-Box Planning Using Macro-Actions with Focused Effects</seealso>)
    /// Do note, this is modified to work with normal classical planning
    /// </summary>
    public class GreedyBFSFocused : BaseHeuristicPlanner
    {
        /// <summary>
        /// Logging event for the front end
        /// </summary>
        public override event LogEventHandler? DoLog;

        private readonly int _numberOfMacros = 8;
        private readonly int _searchBudget = 1;
        private readonly int _parameterLimit = 5;
        private List<MacroDecl> _learnedMacros;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        /// <param name="numberOfMacros"></param>
        /// <param name="searchBudget"></param>
        /// <param name="parameterLimit"></param>
        public GreedyBFSFocused(IHeuristic heuristic, int numberOfMacros, int searchBudget, int parameterLimit) : base(heuristic)
        {
            _numberOfMacros = numberOfMacros;
            _searchBudget = searchBudget;
            _learnedMacros = new List<MacroDecl>();
            _parameterLimit = parameterLimit;
        }

        internal override ActionPlan? Solve()
        {
            DoLog?.Invoke($"Finding focused macros...");
            _learnedMacros = LearnFocusedMacros(_numberOfMacros, _searchBudget);
            DoLog?.Invoke($"Found {_learnedMacros.Count} macros!");
            DoLog?.Invoke($"Retranslating...");
            _context.PDDL.Domain.Actions.AddRange(_learnedMacros.Select(x => x.Macro));
            var translator = new PDDLToSASTranslator(false);
            translator.TimeLimit = TimeSpan.FromSeconds(1000);
            _context = translator.Translate(_context.PDDL);
            DoLog?.Invoke($"Searching...");

            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                foreach (var opID in _context.ApplicabilityGraph[stateMove.Operator])
                {
                    var op = _context.SAS.GetOperatorByID(opID);
                    if (Abort) break;
                    if (stateMove.State.IsApplicable(op))
                    {
                        var newMove = GenerateNewState(stateMove, op);
                        if (!IsVisited(newMove))
                        {
                            var value = Heuristic.GetValue(stateMove, newMove.State, _context.SAS.Operators);
                            newMove.hValue = value;
                            QueueOpenList(stateMove, newMove, op);
                            if (newMove.State.IsInGoal())
                                return GeneratePlanChainWithoutMacros(newMove);
                        }
                    }
                }
            }
            return null;
        }

        private ActionPlan GeneratePlanChainWithoutMacros(StateMove state)
        {
            var macroOps = _context.SAS.Operators.Where(x => _learnedMacros.Any(y => y.Macro.Name == x.Name)).ToList();
            var chain = new List<GroundedAction>();

            while (_planMap.ContainsKey(state))
            {
                if (state.Operator == uint.MaxValue)
                    break;
                var previousState = _planMap[state];
                var macroOp = macroOps.FirstOrDefault(x => x.ID == state.Operator);
                if (macroOp != null)
                {
                    var macro = _learnedMacros.First(x => x.Macro.Name == macroOp.Name);
                    var nameDict = new Dictionary<string, string>();
                    for (int i = 0; i < macroOp.Arguments.Length; i++)
                        nameDict.Add(macro.Macro.Parameters.Values[i].Name, macroOp.Arguments[i]);
                    foreach (var macroStep in macro.Replacements)
                    {
                        var newAct = new GroundedAction(macroStep.Name);
                        foreach (var arg in macroStep.Parameters.Values)
                            newAct.Arguments.Add(new NameExp(nameDict[arg.Name]));
                        chain.Add(newAct);
                    }
                }
                else
                    chain.Add(GenerateFromOp(_context.SAS.Operators.First(x => x.ID == state.Operator)));
                state = previousState;
            }
            chain.Reverse();

            return new ActionPlan(chain);
        }

        private List<uint> GeneratePlanOperatorChain(StateMove state)
        {
            var chain = new List<uint>();

            while (_planMap.ContainsKey(state))
            {
                if (state.Operator == uint.MaxValue)
                    break;
                chain.Add(state.Operator);
                state = _planMap[state];
            }
            chain.Reverse();

            return chain;
        }

        // Based on Algorithm 1 from the paper (for black box planning)
        // Note, the repetition step is left out, since it seemed unnessesary and complicated to make with this system (have to constantly retranslate)
        private List<MacroDecl> LearnFocusedMacros(int nMacros, int budget)
        {
            var newDecl = _context.SAS.Copy();
            var returnMacros = new List<MacroDecl>();

            if (Abort) return new List<MacroDecl>();
            var queue = new FixedMaxPriorityQueue<MacroDecl>(nMacros);
            var h = new EffectHeuristic(new SASStateSpace(new TranslatorContext(_context) { SAS = newDecl }));
            var g = new hPath();

            // Explore state space
            var planner = new GreedyBFS(new hColSum(new List<IHeuristic>() { g, h }));
            planner.TimeLimit = TimeSpan.FromSeconds(budget);
            planner.Solve(new TranslatorContext(_context) { SAS = newDecl });
            foreach (var state in planner._closedList)
            {
                if (Abort) return new List<MacroDecl>();
                var plan = GeneratePlanOperatorChain(state);
                if (plan.Count > 1)
                    queue.Enqueue(
                        GenerateMacroFromOperatorSteps(plan),
                        h.GetValue(new StateMove(new SASStateSpace(new TranslatorContext(_context) { SAS = new SASDecl() })), state.State, new List<Operator>()));
            }

            // Add unique macros
            int added = 0;
            while (added < nMacros && queue.Count > 0)
            {
                if (Abort) return new List<MacroDecl>();
                var newMacro = queue.Dequeue();
                if (newMacro.Macro.Parameters.Values.Count > _parameterLimit)
                    continue;
                if (!returnMacros.Contains(newMacro))
                {
                    returnMacros.Add(newMacro);
                    added++;
                }
            }

            for (int i = 0; i < returnMacros.Count; i++)
                returnMacros[i].Macro.Name = $"{returnMacros[i].Macro.Name}_{i}";

            return returnMacros;
        }

        // This section is mostly based on Algorithm 2 from appendix
        // Its quite slow, but this search algorithm is mostly to show the possibilities of reducing generated states, not search time.
        private MacroDecl GenerateMacroFromOperatorSteps(List<uint> steps)
        {
            var actionCombiner = new ActionDeclCombiner();

            var actionChain = new List<ActionDecl>();
            // Convert operator steps into pddl actions
            foreach (var id in steps)
            {
                var step = _context.SAS.GetOperatorByID(id);
                var newAct = _context.PDDL.Domain.Actions.Single(x => x.Name == step.Name).Copy();
                for (int j = 0; j < newAct.Parameters.Values.Count; j++)
                {
                    var allRefs = newAct.FindNames(newAct.Parameters.Values[j].Name);
                    foreach (var refName in allRefs)
                        refName.Name = $"?{step.Arguments[j]}";
                }
                actionChain.Add(newAct);
            }
            // Generate macro from action chain
            var macro = actionCombiner.Combine(actionChain);
            return new MacroDecl(macro, actionChain);
        }

        private class EffectHeuristic : BaseHeuristic
        {
            private readonly SASStateSpace _initial;
            public EffectHeuristic(SASStateSpace initial)
            {
                _initial = initial;
            }

            internal override uint GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
            {
                uint changed = 0;
                foreach (var item in _initial)
                    if (!state[item])
                        changed++;
                foreach (var item in state)
                    if (!_initial[item])
                        changed++;
                if (changed > 0)
                    return changed;
                return int.MaxValue;
            }
        }

        private class MacroDecl
        {
            public ActionDecl Macro { get; set; }
            public List<ActionDecl> Replacements { get; set; }

            public MacroDecl(ActionDecl macro, List<ActionDecl> replacements)
            {
                Macro = macro;
                Replacements = replacements;
            }
        }
    }
}
