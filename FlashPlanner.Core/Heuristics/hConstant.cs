using FlashPlanner.Core.Models;
using FlashPlanner.Core.Models.SAS;
using FlashPlanner.Core.States;

namespace FlashPlanner.Core.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">constant Evaluator</seealso>
    /// </summary>
    public class hConstant : BaseHeuristic
    {
        /// <summary>
        /// What constant to return
        /// </summary>
        public uint Constant { get; }

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="constant"></param>
        public hConstant(uint constant)
        {
            Constant = constant;
        }

        internal override uint GetValueInner(StateMove parent, SASStateSpace state, List<Operator> operators) => Constant;
    }
}
