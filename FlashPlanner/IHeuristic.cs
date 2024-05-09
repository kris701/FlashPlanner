using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;

namespace FlashPlanner
{
    public interface IHeuristic
    {
        public int Evaluations { get; }
        public int GetValue(StateMove parent, ISASState state, List<Operator> operators);
        public void Reset();
    }
}
