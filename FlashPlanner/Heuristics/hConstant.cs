using FlashPlanner.States;
using FlashPlanner.Tools;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">constant Evaluator</seealso>
    /// </summary>
    public class hConstant : BaseHeuristic
    {
        /// <summary>
        /// What constant to return
        /// </summary>
        public int Constant { get; }

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="constant"></param>
        public hConstant(int constant)
        {
            Constant = constant;
        }

        internal override int GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators) => Constant;
    }
}
