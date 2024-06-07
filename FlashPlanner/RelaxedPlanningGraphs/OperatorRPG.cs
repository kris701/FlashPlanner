using FlashPlanner.Models.SAS;
using FlashPlanner.States;

namespace FlashPlanner.RelaxedPlanningGraphs
{
    /// <summary>
    /// Operator Relaxed Planning Graphs
    /// </summary>
    public class OperatorRPG : BaseRPG
    {
        /// <summary>
        /// Bool indicating if the generation failed
        /// </summary>
        public bool Failed { get; internal set; } = false;

        private SASDecl _currentDecl = new SASDecl();
        private Dictionary<int, List<Operator>> _addOps = new Dictionary<int, List<Operator>>();

        /// <summary>
        /// Generate a relaxed plan
        /// </summary>
        /// <param name="state"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        public List<Operator> GenerateReplaxedPlan(SASStateSpace state, List<Operator> operators)
        {
            if (state.Context.SAS != _currentDecl)
            {
                _currentDecl = state.Context.SAS;
                GenerateAddCache();
            }

            Failed = false;
            var relaxedState = new RelaxedSASStateSpace(state);

            var graphLayers = GenerateRelaxedPlanningGraph(relaxedState, operators);
            if (graphLayers.Count == 0)
            {
                Failed = true;
                return new List<Operator>();
            }
            var selectedOperators = ReconstructPlan(graphLayers, relaxedState.Context.SAS);

            return selectedOperators;
        }

        /// <summary>
        /// Generate a cache of what add facts each operator has.
        /// This is useful when it comes to reconstructing plans.
        /// </summary>
        private void GenerateAddCache()
        {
            _addOps = new Dictionary<int, List<Operator>>();
            foreach (var op in _currentDecl.Operators)
            {
                foreach (var add in op.Add)
                {
                    if (!_addOps.ContainsKey(add.ID))
                        _addOps.Add(add.ID, new List<Operator>());
                    _addOps[add.ID].Add(op);
                }
            }
        }

        // Hoffman & Nebel 2001, Figure 2
        private List<Operator> ReconstructPlan(List<Layer> graphLayers, SASDecl decl)
        {
            var selectedOperators = new List<Operator>();
            var G = new Dictionary<int, HashSet<int>>();
            var trues = new Dictionary<int, HashSet<int>>();
            var m = -1;
            foreach (var fact in decl.Goal)
                m = Math.Max(m, FirstLevel(fact, graphLayers));

            G.Add(0, new HashSet<int>());
            trues.Add(0, new HashSet<int>());
            for (int t = 1; t <= m; t++)
            {
                G.Add(t, new HashSet<int>());
                trues.Add(t, new HashSet<int>());
                foreach (var fact in decl.Goal)
                    if (FirstLevel(fact, graphLayers) == t)
                        G[t].Add(fact.ID);
            }

            for (int i = m; i > 0; i--)
            {
                foreach (var factID in G[i])
                {
                    if (trues[i].Contains(factID))
                        continue;

                    var options = new PriorityQueue<Operator, int>();
                    foreach (var op in _addOps[factID])
                    {
                        if (!graphLayers[i - 1].Operators.ContainsKey(op.ID))
                            continue;
                        var diff = Difficulty(op, graphLayers);
                        options.Enqueue(op, diff);
                        if (diff == 0)
                            break;
                    }

                    if (options.Count > 0)
                    {
                        var best = options.Dequeue();
                        selectedOperators.Add(best);
                        foreach (var pre in best.Pre)
                        {
                            var targetLayer = FirstLevel(pre, graphLayers);
                            if (targetLayer == i || trues[i - 1].Contains(pre.ID))
                                continue;
                            G[targetLayer].Add(pre.ID);
                        }

                        foreach (var add in best.Add)
                        {
                            trues[i - 1].Add(add.ID);
                            trues[i].Add(add.ID);
                        }
                    }
                }
            }

            return selectedOperators;
        }

        // Hoffman & Nebel 2001, Equation 4
        private int Difficulty(Operator op, List<Layer> graphLayers)
        {
            int diff = int.MaxValue;
            foreach (var pre in op.Pre)
                diff = Math.Min(diff, FirstLevel(pre, graphLayers));
            return diff;
        }

        private int FirstLevel(Fact fact, List<Layer> layers)
        {
            for (int i = 0; i < layers.Count; i++)
                if (layers[i].Propositions.Contains(fact.ID))
                    return i;
            throw new Exception();
        }

        /// <summary>
        /// Generate a relaxed planning graph
        /// </summary>
        /// <param name="state"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        public List<Layer> GenerateRelaxedPlanningGraph(RelaxedSASStateSpace state, List<Operator> operators)
        {
            state = new RelaxedSASStateSpace(state);
            bool[] covered = new bool[operators.Count];
            List<Layer> layers = new List<Layer>();
            var newLayer = new Layer(
                GetNewApplicableOperators(state, operators, covered),
                state.GetFacts());
            layers.Add(newLayer);
            int previousLayer = 0;
            while (!state.IsInGoal())
            {
                // Apply applicable actions to state
                state = new RelaxedSASStateSpace(state, layers[previousLayer].Operators.Values.ToList());

                if (state.Count == layers[previousLayer].Propositions.Count)
                    return new List<Layer>();

                newLayer = new Layer(
                    GetNewApplicableOperators(state, operators, covered),
                    state.GetFacts());

                // Error condition: there are no applicable actions at all (most likely means the problem is unsolvable)
                if (newLayer.Operators.Count == 0 && !state.IsInGoal())
                    return new List<Layer>();

                previousLayer++;

                // Add new layer
                layers.Add(newLayer);
            }
            return layers;
        }
    }
}
