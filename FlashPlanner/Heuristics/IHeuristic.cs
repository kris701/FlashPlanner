using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;


namespace FlashPlanner.Heuristics
{
    public interface IHeuristic
    {
        public int Evaluations { get; }
        public int GetValue(StateMove parent, SASStateSpace state, List<Operator> operators);
        public void Reset();
    }
}
