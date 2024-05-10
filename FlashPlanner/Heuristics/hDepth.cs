using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;


namespace FlashPlanner.Heuristics
{
    /// <summary>
    /// Simply forces the search to be a depth first search
    /// </summary>
    public class hDepth : BaseHeuristic
    {
        public override int GetValue(StateMove parent, SASStateSpace state, List<Operator> operators)
        {
            Evaluations++;
            return parent.hValue - 1;
        }
    }
}
