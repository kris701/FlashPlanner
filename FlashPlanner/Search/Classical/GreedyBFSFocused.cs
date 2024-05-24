using FlashPlanner.Heuristics;
using FlashPlanner.HeuristicsCollections;
using FlashPlanner.States;
using FlashPlanner.Tools;
using FlashPlanner.Translators;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkits;

namespace FlashPlanner.Search.Classical
{
    /// <summary>
    /// Greedy Best First Search with Focused Macros.
    /// (<seealso href="https://arxiv.org/abs/2004.13242">Efficient Black-Box Planning Using Macro-Actions with Focused Effects</seealso>)
    /// Do note, this is modified to work with normal classical planning
    /// </summary>
    public class GreedyBFSFocused : BaseClassicalSearch
    {
        public override event LogEventHandler? DoLog;

        public int NumberOfMacros { get; set; } = 8;
        public int SearchBudget { get; set; } = 1;
        public List<ActionDecl> LearnedMacros { get; set; } = new List<ActionDecl>();

        private readonly PDDLDecl _pddlDecl;

        public GreedyBFSFocused(PDDLDecl pddlDecl, SASDecl decl, IHeuristic heuristic, int numberOfMacros, int searchBudget) : base(decl, heuristic)
        {
            _pddlDecl = pddlDecl.Copy();
            NumberOfMacros = numberOfMacros;
            SearchBudget = searchBudget;
        }

        internal override ActionPlan? Solve(IHeuristic h, SASStateSpace state)
        {
            DoLog?.Invoke($"Finding focused macros...");
            LearnedMacros = LearnFocusedMacros(NumberOfMacros, SearchBudget);
            DoLog?.Invoke($"Found {LearnedMacros.Count} macros!");
            DoLog?.Invoke($"Retranslating...");
            _pddlDecl.Domain.Actions.AddRange(LearnedMacros);
            var translator = new PDDLToSASTranslator(true, false);
            translator.TimeLimit = TimeSpan.FromSeconds(1000);
            Declaration = translator.Translate(_pddlDecl);
            DoLog?.Invoke($"Searching...");

            while (!Abort && _openList.Count > 0)
            {
                var stateMove = ExpandBestState();
                foreach (var op in Declaration.Operators)
                {
                    if (Abort) break;
                    if (stateMove.State.IsApplicable(op))
                    {
                        var newMove = GenerateNewState(stateMove, op);
                        if (newMove.State.IsInGoal())
                            return GeneratePlanChain(stateMove);
                        if (!IsVisited(newMove))
                        {
                            var value = h.GetValue(stateMove, newMove.State, Declaration.Operators);
                            newMove.hValue = value;
                            _openList.Enqueue(newMove, value);
                        }
                    }
                }
            }
            return null;
        }


        // Based on Algorithm 1 from the paper (for black box planning)
        // Note, the repetition step is left out, since it seemed unnessesary and complicated to make with this system (have to constantly retranslate)
        private List<ActionDecl> LearnFocusedMacros(int nMacros, int budget)
        {
            var newDecl = Declaration.Copy();
            var returnMacros = new List<ActionDecl>();

            if (Abort) return new List<ActionDecl>();
            var queue = new FixedMaxPriorityQueue<ActionDecl>(nMacros);
            var h = new EffectHeuristic(new SASStateSpace(newDecl));
            var g = new hPath();

            // Explore state space
            using (var search = new Classical.GreedyBFS(newDecl, new hColSum(new List<IHeuristic>() { g, h })))
            {
                search.TimeLimit = TimeSpan.FromSeconds(budget);
                search.Solve();
                foreach (var state in search._closedList)
                {
                    if (Abort) return new List<ActionDecl>();
                    if (state.PlanSteps.Count > 0)
                        queue.Enqueue(
                            GenerateMacroFromOperatorSteps(state.PlanSteps),
                            h.GetValue(new StateMove(), state.State, new List<Operator>()));
                }
            }

            // Add unique macros
            int added = 0;
            while (added < nMacros && queue.Count > 0)
            {
                if (Abort) return new List<ActionDecl>();
                var newMacro = queue.Dequeue();
                if (!returnMacros.Contains(newMacro))
                {
                    returnMacros.Add(newMacro);
                    added++;
                }
            }

            for (int i = 0; i < returnMacros.Count; i++)
                returnMacros[i].Name = $"{returnMacros[i].Name}_{i}";

            return returnMacros;
        }

        // This section is mostly based on Algorithm 2 from appendix
        // Its quite slow, but this search algorithm is mostly to show the possibilities of reducing generated states, not search time.
        private ActionDecl GenerateMacroFromOperatorSteps(List<int> steps)
        {
            var actionCombiner = new ActionDeclCombiner();

            var actionChain = new List<ActionDecl>();
            // Convert operator steps into pddl actions
            foreach (var id in steps)
            {
                var step = Declaration.GetOperatorByID(id);
                var newAct = _pddlDecl.Domain.Actions.First(x => x.Name == step.Name).Copy();
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
            // Anonymize the parameters of the macro
            AnonymizeActionDecl(macro);
            return macro;
        }

        private void AnonymizeActionDecl(ActionDecl action)
        {
            for (int j = 0; j < action.Parameters.Values.Count; j++)
            {
                var allRefs = action.FindNames(action.Parameters.Values[j].Name);
                foreach (var refName in allRefs)
                    refName.Name = $"?{j}";
            }
        }

        private class EffectHeuristic : BaseHeuristic
        {
            private readonly SASStateSpace _initial;
            public EffectHeuristic(SASStateSpace initial)
            {
                _initial = initial;
            }

            public override int GetValue(StateMove parent, SASStateSpace state, List<Operator> operators)
            {
                Evaluations++;
                var changed = 0;
                foreach (var item in _initial._state)
                    if (!state.Contains(item))
                        changed++;
                foreach (var item in state._state)
                    if (!_initial.Contains(item))
                        changed++;
                if (changed > 0)
                    return changed;
                return int.MaxValue;
            }
        }
    }
}
