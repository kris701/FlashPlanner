using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;

namespace FlashPlanner.Translators.Helpers
{
    public static class ExpressionHelper
    {
        public static IExp EnsureAnd(IExp exp)
        {
            if (exp is AndExp)
                return exp;
            return new AndExp(new List<IExp>() { exp });
        }
    }
}
