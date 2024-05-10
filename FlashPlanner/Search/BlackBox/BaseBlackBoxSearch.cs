using FlashPlanner.Heuristics;
using FlashPlanner.Search.Classical;
using FlashPlanner.States;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Search.BlackBox
{
    public abstract class BaseBlackBoxSearch : BaseClassicalSearch
    {
        public BaseBlackBoxSearch(SASDecl decl, IHeuristic heuristic) : base(decl, heuristic)
        {
            switch (heuristic)
            {
                case hGoal: break;
                default:
                    throw new Exception("Invalid heuristic for black box planner!");
            }
        }

        public List<int> GetApplicables(SASStateSpace state)
        {
            var returnList = new List<int>();
            for (int i = 0; i < Declaration.Operators.Count; i++)
            {
                if (Aborted) break;
                if (state.IsApplicable(Declaration.Operators[i]))
                    returnList.Add(i);
            }
            return returnList;
        }

        public SASStateSpace Simulate(SASStateSpace state, int opIndex)
        {
            Generated++;
            var newState = new SASStateSpace(state);
            newState.Execute(Declaration.Operators[opIndex]);
            return newState;
        }

        internal List<GroundedAction> GeneratePlanChain(List<Operator> steps, int newOp)
        {
            var chain = new List<GroundedAction>();

            chain.AddRange(GeneratePlanChain(steps));
            chain.Add(GenerateFromOp(newOp));

            return chain;
        }

        internal GroundedAction GenerateFromOp(int opIndex) => new GroundedAction(Declaration.Operators[opIndex].Name, Declaration.Operators[opIndex].Arguments);
    }
}
