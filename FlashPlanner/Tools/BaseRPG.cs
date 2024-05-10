using FlashPlanner.States;
using PDDLSharp.Models.SAS;


namespace FlashPlanner.Tools
{
    public abstract class BaseRPG
    {
        internal List<Operator> GetNewApplicableOperators(SASStateSpace state, List<Operator> operators, bool[] covered) => GetNewApplicableOperators(state, new List<Operator>(), operators, covered);
        internal List<Operator> GetNewApplicableOperators(SASStateSpace state, List<Operator> from, List<Operator> operators, bool[] covered)
        {
            var result = new List<Operator>(from);
            for (int i = 0; i < covered.Length; i++)
            {
                if (!covered[i] && state.IsNodeTrue(operators[i]))
                {
                    result.Add(operators[i]);
                    covered[i] = true;
                }
            }
            return result;
        }
    }
}
