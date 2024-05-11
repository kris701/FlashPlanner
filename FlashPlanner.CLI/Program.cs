using CommandLine;
using CommandLine.Text;
using PDDLSharp.CodeGenerators.FastDownward.Plans;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.PDDL;

namespace FlashPlanner.CLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<Options>(args);
            parserResult.WithNotParsed(errs => DisplayHelp(parserResult, errs));
            parserResult.WithParsed(Run);
        }

        public static void Run(Options opts)
        {
            opts.DomainPath = RootPath(opts.DomainPath);
            opts.ProblemPath = RootPath(opts.ProblemPath);
            if (opts.PlanPath == "")
                opts.PlanPath = "solution.plan";
            opts.PlanPath = RootPath(opts.PlanPath);

            Console.WriteLine("Parsing files...");
            var listener = new ErrorListener();
            var pddlParser = new PDDLParser(listener);
            var domain = pddlParser.ParseAs<DomainDecl>(new FileInfo(opts.DomainPath));
            var problem = pddlParser.ParseAs<ProblemDecl>(new FileInfo(opts.ProblemPath));
            var pddlDecl = new PDDLDecl(domain, problem);

            Console.WriteLine("Building translator...");
            var translator = InputArgumentBuilder.GetTranslator(pddlDecl, opts.TranslatorOption);
            if (opts.TranslatorTimeLimit > 0)
                translator.TimeLimit = TimeSpan.FromSeconds(opts.TranslatorTimeLimit);

            Console.WriteLine("Translating...");
            var sasDecl = translator.Translate(pddlDecl);

            if (translator.Aborted)
            {
                Console.WriteLine("Translator Timed Out!...");
                return;
            }

            Console.WriteLine("Building search engine...");
            using (var planner = InputArgumentBuilder.GetPlanner(pddlDecl, sasDecl, opts.SearchOption))
            {
                if (opts.SearchTimeLimit > 0)
                    planner.TimeLimit = TimeSpan.FromSeconds(opts.SearchTimeLimit);
                planner.Log = true;
                var solution = planner.Solve();

                if (planner.Aborted)
                    Console.WriteLine("Planner timed out!");
                else
                {
                    Console.WriteLine("Planner succeded!");
                    var planGenerator = new FastDownwardPlanGenerator(listener);
                    var plan = planGenerator.Generate(solution);
                    Console.WriteLine("Plan:");
                    Console.WriteLine(plan);

                    File.WriteAllText(opts.PlanPath, plan);
                }
            }
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            var sentenceBuilder = SentenceBuilder.Create();
            foreach (var error in errs)
                if (error is not HelpRequestedError)
                    Console.WriteLine(sentenceBuilder.FormatError(error));
        }

        private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AddEnumValuesToHelpText = true;
                return h;
            }, e => e, verbsIndex: true);
            Console.WriteLine(helpText);
            HandleParseError(errs);
        }

        private static string RootPath(string path)
        {
            if (!Path.IsPathRooted(path))
                path = Path.Join(Directory.GetCurrentDirectory(), path);
            path = path.Replace("\\", "/");
            return path;
        }
    }
}