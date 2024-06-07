using FlashPlanner.Models;
using FlashPlanner.States;
using PDDLSharp.Models.SAS;


namespace FlashPlanner.Heuristics
{
    /// <summary>
    /// Main interface for heuristics
    /// </summary>
    public interface IHeuristic
    {
        /// <summary>
        /// How many times the <seealso cref="GetValue(StateMove, SASStateSpace, List{Operator})"/> method have been called
        /// </summary>
        public int Evaluations { get; }
        /// <summary>
        /// Get a heuristic value for the current <paramref name="state"/>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="state"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        public int GetValue(StateMove parent, SASStateSpace state, List<Operator> operators);
        /// <summary>
        /// Reset the heuristic.
        /// </summary>
        public void Reset();
    }
}
