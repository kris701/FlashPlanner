using FlashPlanner.Translators.Exceptions;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Overloads;

namespace FlashPlanner.Translators.Components
{
    /// <summary>
    /// A class able to deconstruct <seealso cref="OrExp"/> into simpler formats.
    /// </summary>
    public class OrDeconstructor
    {
        /// <summary>
        /// Bool representing if the deconstructor should stop or not
        /// </summary>
        public bool Aborted { get; set; } = false;

        /// <summary>
        /// Takes an action and makes simplified versions of it.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public List<ActionDecl> DeconstructOrs(ActionDecl action)
        {
            var deconstructed = new List<ActionDecl>();
            deconstructed.AddRange(GeneratePossibleActions(action.Copy()));
            return deconstructed;
        }

        private List<ActionDecl> GeneratePossibleActions(ActionDecl source)
        {
            source.EnsureAnd();
            var returnList = new List<ActionDecl>();
            DeconstructNodeRec(source, returnList);
            if (Aborted) return new List<ActionDecl>();
            returnList = returnList.Distinct().ToList();
            return returnList;
        }

        private void DeconstructNodeRec(ActionDecl act, List<ActionDecl> returnList)
        {
            if (Aborted) return;
            var ors = act.Preconditions.FindTypes<OrExp>();
            if (ors.Any(x => x.Options.Count == 0 || x.Options.Count == 1))
                throw new TranslatorException("Unknown error occured!");
            if (ors.Count <= 0)
            {
                returnList.Add(act);
                return;
            }

            int deepestIndex = GetDeepestNodeIndex(ors, act.Preconditions);

            foreach (var opt in ors[deepestIndex].Options)
            {
                if (Aborted) return;
                var copy = act.Copy();
                var optCopy = opt.Copy(ors[deepestIndex].Parent);
                var or = copy.Preconditions.FindTypes<OrExp>();
                if (or[deepestIndex].Parent is IWalkable walk)
                    walk.Replace(or[deepestIndex], optCopy);

                if (or.Count == 1)
                    returnList.Add(copy);
                else
                    DeconstructNodeRec(copy, returnList);
            }
        }

        private int GetDeepestNodeIndex<T>(List<T> nodes, INode until) where T : INode
        {
            int deepest = -1;
            int deepestIndex = -1;
            for (int i = 0; i < nodes.Count; i++)
            {
                var depth = Depth(nodes[i], until);
                if (depth > deepest)
                {
                    deepest = depth;
                    deepestIndex = i;
                }
            }
            return deepestIndex;
        }

        private int Depth(INode current, INode until)
        {
            if (current == until) return 0;
            if (current.Parent == null) return 0;
            return 1 + Depth(current.Parent, until);
        }
    }
}
