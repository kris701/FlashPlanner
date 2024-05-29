using FlashPlanner.States;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Tools
{
    /// <summary>
    /// Base class for generating relaxed planning graphs
    /// </summary>
    public abstract class BaseRPG
    {
        internal HashSet<Operator> GetNewApplicableOperators(SASStateSpace state, List<Operator> operators, bool[] covered)
        {
            var result = new HashSet<Operator>();
            for (int i = 0; i < covered.Length; i++)
            {
                if (!covered[i] && state.IsApplicable(operators[i]))
                {
                    result.Add(operators[i]);
                    covered[i] = true;
                }
            }
            return result;
        }
    }
}
