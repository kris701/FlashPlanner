using FlashPlanner.States;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Tools
{
    /// <summary>
    /// Fact Relaxed Planning Graph
    /// </summary>
    public class FactRPG : BaseRPG
    {
        /// <summary>
        /// Generate a relaxed planning graph
        /// </summary>
        /// <param name="state"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        public Dictionary<int, int> GenerateRelaxedGraph(SASStateSpace state, List<Operator> operators)
        {
            state = new RelaxedSASStateSpace(state);
            bool[] covered = new bool[operators.Count];
            var dict = new Dictionary<int, int>();
            foreach (var fact in state)
                dict.Add(fact, 0);

            int layer = 1;
            while (!state.IsInGoal())
            {
                var apply = GetNewApplicableOperators(state, operators, covered);
                if (apply.Count == 0)
                    return dict;

                state = new RelaxedSASStateSpace(state);
                int changed = state.Count;
                foreach (var op in apply)
                {
                    state.Execute(op);
                    foreach (var add in op.Add)
                        if (!dict.ContainsKey(add.ID))
                            dict.Add(add.ID, layer);
                }
                if (changed == state.Count)
                    return dict;

                layer++;
            }

            return dict;
        }
    }
}
