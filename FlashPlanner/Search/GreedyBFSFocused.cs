using FlashPlanner.Heuristics;
using FlashPlanner.HeuristicsCollections;
using FlashPlanner.States;
using FlashPlanner.Tools;
using FlashPlanner.Translators;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkits;

namespace FlashPlanner.Search
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
        private readonly PDDLDecl _pddlDecl;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="heuristic"></param>
        /// <param name="pddlDecl"></param>
        /// <param name="numberOfMacros"></param>
        /// <param name="searchBudget"></param>
        /// <param name="parameterLimit"></param>
        public GreedyBFSFocused(IHeuristic heuristic, PDDLDecl pddlDecl, int numberOfMacros, int searchBudget, int parameterLimit) : base(heuristic)
        {
            _pddlDecl = pddlDecl.Copy();
            _numberOfMacros = numberOfMacros;
            _searchBudget = searchBudget;
            _learnedMacros = new List<MacroDecl>();
            _parameterLimit = parameterLimit;
        }

        internal override ActionPlan? Solve(SASStateSpace state)
        {
            DoLog?.Invoke($"Finding focused macros...");
            _learnedMacros = LearnFocusedMacros(_numberOfMacros, _searchBudget);
            DoLog?.Invoke($"Found {_learnedMacros.Count} macros!");
            DoLog?.Invoke($"Retranslating...");
            _pddlDecl.Domain.Actions.AddRange(_learnedMacros.Select(x => x.Macro));
            var translator = new PDDLToSASTranslator(true, false);
            translator.TimeLimit = TimeSpan.FromSeconds(1000);
            _declaration = translator.Translate(_pddlDecl);
            DoLog?.Invoke($"Searching...");

            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                foreach (var op in _declaration.Operators)
                {
                    if (Abort) break;
                    if (stateMove.State.IsApplicable(op))
                    {
                        var newMove = GenerateNewState(stateMove, op);
                        if (newMove.State.IsInGoal())
                            return GeneratePlanChainWithoutMacros(newMove);
                        if (!IsVisited(newMove))
                        {
                            var value = Heuristic.GetValue(stateMove, newMove.State, _declaration.Operators);
                            newMove.hValue = value;
                            _openList.Enqueue(newMove, value);
                        }
                    }
                }
            }
            return null;
        }

        private ActionPlan GeneratePlanChainWithoutMacros(StateMove state)
        {
            var chain = new List<GroundedAction>();
            var macroOps = _declaration.Operators.Where(x => _learnedMacros.Any(y => y.Macro.Name == x.Name)).ToList();

            foreach (var step in state.PlanSteps)
            {
                var macroOp = macroOps.FirstOrDefault(x => x.ID == step);
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
                    chain.Add(GenerateFromOp(_declaration.Operators.First(x => x.ID == step)));
            }

            return new ActionPlan(chain);
        }

        // Based on Algorithm 1 from the paper (for black box planning)
        // Note, the repetition step is left out, since it seemed unnessesary and complicated to make with this system (have to constantly retranslate)
        private List<MacroDecl> LearnFocusedMacros(int nMacros, int budget)
        {
            var newDecl = _declaration.Copy();
            var returnMacros = new List<MacroDecl>();

            if (Abort) return new List<MacroDecl>();
            var queue = new FixedMaxPriorityQueue<MacroDecl>(nMacros);
            var h = new EffectHeuristic(new SASStateSpace(newDecl));
            var g = new hPath();

            // Explore state space
            var planner = new GreedyBFS(new hColSum(new List<IHeuristic>() { g, h }));
            planner.TimeLimit = TimeSpan.FromSeconds(budget);
            planner.Solve(newDecl);
            foreach (var state in planner._closedList)
            {
                if (Abort) return new List<MacroDecl>();
                if (state.PlanSteps.Count > 1)
                    queue.Enqueue(
                        GenerateMacroFromOperatorSteps(state.PlanSteps),
                        h.GetValue(new StateMove(new SASStateSpace(new SASDecl())), state.State, new List<Operator>()));
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
        private MacroDecl GenerateMacroFromOperatorSteps(List<int> steps)
        {
            var actionCombiner = new ActionDeclCombiner();

            var actionChain = new List<ActionDecl>();
            // Convert operator steps into pddl actions
            foreach (var id in steps)
            {
                var step = _declaration.GetOperatorByID(id);
                var newAct = _pddlDecl.Domain.Actions.Single(x => x.Name == step.Name).Copy();
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

            internal override int GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators)
            {
                var changed = 0;
                foreach (var item in _initial)
                    if (!state.Contains(item))
                        changed++;
                foreach (var item in state)
                    if (!_initial.Contains(item))
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
