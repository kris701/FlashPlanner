using FlashPlanner.Core.Translators.Exceptions;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Translators.Grounders;

namespace FlashPlanner.Core.Translators.Normalizers
{
    /// <summary>
    /// A class able to deconstruct <seealso cref="ExistsExp"/> into simpler formats.
    /// </summary>
    public class ExistsNormalizer
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
        public ExistsNormalizer(IGrounder<IParametized> grounder)
        {
            Grounder = grounder;
        }

        /// <summary>
        /// Takes in some <seealso cref="INode"/> expression, and simplifies the <seealso cref="ExistsExp"/> expressions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="TranslatorException"></exception>
        public T DeconstructExists<T>(T node) where T : INode
        {
            var copy = node.Copy(node.Parent);
            var exists = copy.FindTypes<ExistsExp>();
            while (exists.Count > 0)
            {
                if (Aborted) break;
                if (exists[0].Parent is IWalkable walk)
                {
                    var result = Grounder.Ground(exists[0]).Cast<ExistsExp>().ToList();
                    if (result.Count == 1)
                    {
                        result[0].Expression.Parent = exists[0].Parent;
                        walk.Replace(exists[0], result[0].Expression);
                    }
                    else if (result.Count > 1)
                    {
                        var newNode = new OrExp(exists[0].Parent);
                        foreach (var item in result)
                        {
                            item.Expression.Parent = newNode;
                            newNode.Add(item.Expression);
                        }
                        walk.Replace(exists[0], newNode);
                    }
                }
                else
                    throw new TranslatorException("Parent for exists deconstruction must be a IWalkable!");
                exists = copy.FindTypes<ExistsExp>();
            }

            return (T)copy;
        }
    }
}
