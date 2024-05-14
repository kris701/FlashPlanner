using FlashPlanner.Translators.Exceptions;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Translators.Grounders;

namespace FlashPlanner.Translators.Components
{
    /// <summary>
    /// A class able to deconstruct <seealso cref="ForAllExp"/> into simpler formats.
    /// </summary>
    public class ForAllDeconstructor
    {
        /// <summary>
        /// The current grounder being used
        /// </summary>
        public IGrounder<IParametized> Grounder { get; }
        /// <summary>
        /// Bool representing if the deconstructor should stop or not
        /// </summary>
        public bool Aborted { get; set; } = false;

        /// <summary>
        /// Constructor with a grounder given
        /// </summary>
        /// <param name="grounder"></param>
        public ForAllDeconstructor(IGrounder<IParametized> grounder)
        {
            Grounder = grounder;
        }

        /// <summary>
        /// Takes in some <seealso cref="INode"/> expression, and simplifies the <seealso cref="ForAllExp"/> expressions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="TranslatorException"></exception>
        public T DeconstructForAlls<T>(T node) where T : INode
        {
            var copy = node.Copy(node.Parent);
            var forAlls = copy.FindTypes<ForAllExp>();
            while (forAlls.Count > 0)
            {
                if (Aborted) break;
                if (forAlls[0].Parent is IWalkable walk)
                {
                    var result = Grounder.Ground(forAlls[0]).Cast<ForAllExp>().ToList();
                    if (result.Count == 1)
                    {
                        result[0].Expression.Parent = forAlls[0].Parent;
                        walk.Replace(forAlls[0], result[0].Expression);
                    }
                    else if (result.Count > 1)
                    {
                        var newNode = new AndExp(forAlls[0].Parent);
                        foreach (var item in result)
                        {
                            item.Expression.Parent = newNode;
                            newNode.Add(item.Expression);
                        }
                        walk.Replace(forAlls[0], newNode);
                    }
                }
                else
                    throw new TranslatorException("Parent for forall deconstruction must be a IWalkable!");
                forAlls = copy.FindTypes<ForAllExp>();
            }

            return (T)copy;
        }
    }
}
