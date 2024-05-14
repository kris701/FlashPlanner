using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Translators.Grounders;

namespace FlashPlanner.Translators.Normalizers
{
    /// <summary>
    /// Combined deconstructor
    /// </summary>
    public class NodeNormalizer
    {
        private readonly OrNormalizer _orDeconstructor;
        private readonly ForAllNormalizer _forAllDeconstructor;
        private readonly ExistsNormalizer _existsDeconstructor;
        private readonly ImplyNormalizer _implyDeconstructor;
        private readonly ConditionalNormalizer _conditionalDeconstructor;

        /// <summary>
        /// Constructor with a grounder given
        /// </summary>
        /// <param name="grounder"></param>
        public NodeNormalizer(IGrounder<IParametized> grounder)
        {
            _orDeconstructor = new OrNormalizer();
            _forAllDeconstructor = new ForAllNormalizer(grounder);
            _existsDeconstructor = new ExistsNormalizer(grounder);
            _implyDeconstructor = new ImplyNormalizer();
            _conditionalDeconstructor = new ConditionalNormalizer();
        }

        /// <summary>
        /// Deconstruct all the complex elements of some <seealso cref="INode"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public T Deconstruct<T>(T item) where T : INode
        {
            if (item.FindTypes<ForAllExp>().Count > 0)
                item = _forAllDeconstructor.DeconstructForAlls(item);
            if (item.FindTypes<ExistsExp>().Count > 0)
                item = _existsDeconstructor.DeconstructExists(item);
            if (item.FindTypes<ImplyExp>().Count > 0)
                item = _implyDeconstructor.DeconstructImplies(item);
            return item;
        }

        /// <summary>
        /// Execute to abort all sub-deconstructors
        /// </summary>
        public void Abort()
        {
            _orDeconstructor.Aborted = true;
            _forAllDeconstructor.Aborted = true;
            _existsDeconstructor.Aborted = true;
            _implyDeconstructor.Aborted = true;
            _conditionalDeconstructor.Aborted = true;
        }

        /// <summary>
        /// Deconstruct all the complex elements of some <seealso cref="ActionDecl"/>
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        public List<ActionDecl> DeconstructAction(ActionDecl act)
        {
            // Initial unary deconstruction
            act = Deconstruct(act);

            // Multiple deconstruction
            var conditionalsDeconstructed = _conditionalDeconstructor.DecontructConditionals(act);

            if (act.FindTypes<OrExp>().Count > 0)
            {
                var orDeconstructed = new List<ActionDecl>();
                foreach (var decon in conditionalsDeconstructed)
                    orDeconstructed.AddRange(_orDeconstructor.DeconstructOrs(decon));
                return orDeconstructed;
            }

            return conditionalsDeconstructed;
        }
    }
}
