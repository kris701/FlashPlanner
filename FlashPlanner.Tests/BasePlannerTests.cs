using FlashPlanner.Models;
using FlashPlanner.Translators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.SAS;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Tools;
using PDDLSharp.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace FlashPlanner.Tests
{
    public class BasePlannerTests
    {
        private static Dictionary<string, TranslatorContext> _declCache = new Dictionary<string, TranslatorContext>();
        internal static TranslatorContext GetTranslatorContext(string domain, string problem)
        {
            if (_declCache.ContainsKey(domain + problem))
                return _declCache[domain + problem];

            IErrorListener listener = new ErrorListener();
            IParser<INode> parser = new PDDLParser(listener);
            var pddlDecl = new PDDLDecl(
                parser.ParseAs<DomainDecl>(new FileInfo(domain)),
                parser.ParseAs<ProblemDecl>(new FileInfo(problem))
                );

            var translator = new PDDLToSASTranslator(true, false);
            var decl = translator.Translate(pddlDecl);

            _declCache.Add(domain + problem, decl);
            return decl;
        }
    }
}
