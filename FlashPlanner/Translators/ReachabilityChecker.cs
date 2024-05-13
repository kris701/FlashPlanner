using FlashPlanner.States;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Translators
{
    /// <summary>
    /// Reachability reduction for translation
    /// </summary>
    public class ReachabilityChecker
    {
        /// <summary>
        /// Get a set of potentially reachable operators, by means of a relaxed plan
        /// </summary>
        /// <param name="decl"></param>
        /// <returns></returns>
        public List<Operator> GetPotentiallyReachableOperators(SASDecl decl)
        {
            bool any = true;
            bool[] covered = new bool[decl.Operators.Count];
            var state = new RelaxedSASStateSpace(decl);
            var applicables = new List<Operator>();
            while (any)
            {
                any = false;
                var applicableNow = new List<Operator>();
                for (int i = 0; i < decl.Operators.Count; i++)
                {
                    if (!covered[i] && state.IsApplicable(decl.Operators[i]))
                    {
                        any = true;
                        covered[i] = true;
                        applicableNow.Add(decl.Operators[i]);
                    }
                }

                foreach (var op in applicableNow)
                    state.Execute(op);
            }

            for (int i = 0; i < decl.Operators.Count; i++)
                if (covered[i])
                    applicables.Add(decl.Operators[i]);

            return applicables;
        }
    }
}
