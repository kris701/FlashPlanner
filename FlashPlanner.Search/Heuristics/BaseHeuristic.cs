using FlashPlanner.Search.Tools;
using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;

namespace FlashPlanner.Search.Heuristics
{
    public abstract class BaseHeuristic : IHeuristic
    {
        public int Evaluations { get; internal set; }
        public abstract int GetValue(StateMove parent, ISASState state, List<Operator> operators);
        public virtual void Reset()
        {
            Evaluations = 0;
        }
    }
}
