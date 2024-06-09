using FlashPlanner.Translators.Exceptions;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;

namespace FlashPlanner.Translators.Normalizers
{
    /// <summary>
    /// A class able to deconstruct <seealso cref="ImplyExp"/> into simpler formats.
    /// </summary>
    public class ImplyNormalizer
    {
        /// <summary>
        /// Bool representing if the deconstructor should stop or not
        /// </summary>
        public bool Aborted { get; set; } = false;

        /// <summary>
        /// Takes in some <seealso cref="INode"/> expression, and simplifies the <seealso cref="ForAllExp"/> expressions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="TranslatorException"></exception>
        public T DeconstructImplies<T>(T node) where T : INode
        {
            var copy = node.Copy(node.Parent);
            var implies = copy.FindTypes<ImplyExp>();

            while (implies.Count > 0)
            {
                if (Aborted) break;
                if (implies[0].Parent is IWalkable walk)
                {
                    var newNode = new OrExp(implies[0].Parent);
                    var notOption = new NotExp(newNode, new EmptyExp());
                    if (implies[0].Antecedent.Copy(notOption) is IExp nExp)
                        notOption.Child = nExp;
                    newNode.Options.Add(notOption);

                    var andOption = new AndExp(newNode, new List<IExp>());
                    if (implies[0].Antecedent.Copy(andOption) is IExp exp1)
                        andOption.Children.Add(exp1);
                    if (implies[0].Consequent.Copy(andOption) is IExp exp2)
                        andOption.Children.Add(exp2);
                    newNode.Options.Add(andOption);

                    walk.Replace(implies[0], newNode);
                }
                else
                    throw new TranslatorException("Parent for imply deconstruction must be a IWalkable!");
                implies = copy.FindTypes<ImplyExp>();
            }

            return (T)copy;
        }
    }
}
