using FlashPlanner.Heuristics;
using FlashPlanner.Search.Classical;
using FlashPlanner.States;
using FlashPlanner.Tools;
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
                if (Abort) break;
                if (state.IsApplicable(Declaration.Operators[i]))
                    returnList.Add(i);
            }
            return returnList;
        }

        public StateMove Simulate(StateMove state, int opIndex)
        {
            Generated++;
            var newState = new SASStateSpace(state.State);
            newState.Execute(Declaration.Operators[opIndex]);
            var stateMove = new StateMove(newState);
            stateMove.PlanSteps.AddRange(state.PlanSteps);
            stateMove.PlanSteps.Add(Declaration.Operators[opIndex].ID);
            return stateMove;
        }
    }
}
