using FlashPlanner.Search.Tools;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;

namespace FlashPlanner.Search
{
    public interface IHeuristic
    {
        public int Evaluations { get; }
        public int GetValue(StateMove parent, ISASState state, List<Operator> operators);
        public void Reset();
    }
}
