using CommandLine;
using CommandLine.Text;
using FlashPlanner.Models;
using PDDLSharp.CodeGenerators.FastDownward.Plans;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.PDDL;
using System.Diagnostics;

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
            if (opts.AliasOption == "" && opts.SearchOption == "")
                throw new Exception("No alias or search provided!");

            if (opts.AliasOption != "")
            {
                var alias = InputArgumentBuilder.GetAlias(opts.AliasOption);
                opts.TranslatorOption = alias.TranslatorOption;
                opts.SearchOption = alias.SearchOption;
            }

            WriteLineColor("Initializing", ConsoleColor.Blue);
            WriteLineColor($"\tDomain File:             {opts.DomainPath}");
            WriteLineColor($"\tProblem File:            {opts.ProblemPath}");
            WriteLineColor($"\tSearch Arguments:        {opts.SearchOption}");
            if (opts.SearchTimeLimit > 0)
                WriteLineColor($"\tSearch Time limit:       {opts.SearchTimeLimit}s");
            if (opts.SearchMemoryLimit > 0)
                WriteLineColor($"\tSearch Memory limit:     {opts.SearchMemoryLimit}MB");
            WriteLineColor($"\tTranslation Arguments:   {opts.TranslatorOption}");
            if (opts.TranslatorTimeLimit > 0)
                WriteLineColor($"\tTranslation Time limit:  {opts.TranslatorTimeLimit}s");
            if (opts.TranslatorMemoryLimit > 0)
                WriteLineColor($"\tTranslation Memory limit:{opts.TranslatorMemoryLimit}MB");

            WriteColor("\tChecking Files...");
            opts.DomainPath = RootPath(opts.DomainPath);
            opts.ProblemPath = RootPath(opts.ProblemPath);
            opts.PlanPath = RootPath(opts.PlanPath);

            if (!File.Exists(opts.DomainPath))
                throw new FileNotFoundException($"Domain file not found: {opts.DomainPath}");
            if (!File.Exists(opts.ProblemPath))
                throw new FileNotFoundException($"Problem file not found: {opts.ProblemPath}");

            WriteLineColor("Done!", ConsoleColor.Green);

            WriteColor("\tParsing Files...");
            var listener = new ErrorListener();
            var pddlParser = new PDDLParser(listener);
            var domain = pddlParser.ParseAs<DomainDecl>(new FileInfo(opts.DomainPath));
            var problem = pddlParser.ParseAs<ProblemDecl>(new FileInfo(opts.ProblemPath));
            var pddlDecl = new PDDLDecl(domain, problem);
            WriteLineColor("Done!", ConsoleColor.Green);

            var sasDecl = Translate(opts, pddlDecl);
            if (sasDecl == null)
                return;

            var plan = Search(opts, pddlDecl, sasDecl);

            if (opts.DoValidate && plan != null)
                Validate(plan, pddlDecl);
        }

        private static TranslatorContext? Translate(Options opts, PDDLDecl pddlDecl)
        {
            WriteLineColor("Translation", ConsoleColor.Blue);

            WriteColor("\tBuilding Translator...");
            var translator = InputArgumentBuilder.GetTranslator(pddlDecl, opts.TranslatorOption);
            if (opts.TranslatorTimeLimit > 0)
                translator.TimeLimit = TimeSpan.FromSeconds(opts.TranslatorTimeLimit);
            if (opts.TranslatorMemoryLimit > 0)
                translator.MemoryLimit = opts.TranslatorMemoryLimit;
            WriteLineColor("Done!", ConsoleColor.Green);

            translator.DoLog += OnLog;
            var watch = new Stopwatch();

            watch.Start();
            WriteLineColor("\tTranslating...");
            var context = translator.Translate(pddlDecl);
            watch.Stop();

            switch (translator.Code)
            {
                case ILimitedComponent.ReturnCode.Success: WriteLineColor("\tTranslation successful!", ConsoleColor.Green); break;
                case ILimitedComponent.ReturnCode.TimedOut: WriteLineColor("\tTranslator timed out...", ConsoleColor.Yellow); return null;
                case ILimitedComponent.ReturnCode.MemoryLimitReached: WriteLineColor("\tTranslator reached memory limit...", ConsoleColor.Yellow); return null;
            }

            WriteLineColor($"\tTranslation took {translator.ExecutionTime.TotalSeconds} seconds");
            WriteLineColor($"\tPeak memory usage: {translator.MemoryUsed}MB");
            WriteLineColor("\tTranslator info:");
            WriteLineColor($"\t\t{context.SAS.Facts} total facts", ConsoleColor.DarkGray);
            WriteLineColor($"\t\t{context.SAS.Operators.Count} operators", ConsoleColor.DarkGray);
            WriteLineColor($"\t\t{context.SAS.Init.Length} initial facts", ConsoleColor.DarkGray);
            WriteLineColor($"\t\t{context.SAS.Goal.Length} goal facts", ConsoleColor.DarkGray);
            return context;
        }

        private static ActionPlan? Search(Options opts, PDDLDecl pddlDecl, TranslatorContext context)
        {
            WriteLineColor("Search", ConsoleColor.Blue);
            WriteColor("\tBuilding Search engine...");
            ActionPlan? solution = null;

            var planner = InputArgumentBuilder.GetPlanner(pddlDecl, opts.SearchOption);
            planner.DoLog += OnLog;
            WriteLineColor("Done!", ConsoleColor.Green);
            var watch = new Stopwatch();

            var logTimer = new System.Timers.Timer();
            logTimer.Interval = 1000;
            logTimer.AutoReset = true;
            logTimer.Elapsed += (s, e) =>
            {
                WriteLineColor($"\t\t[{Math.Round(watch.Elapsed.TotalSeconds, 0)}s] Expanded {planner.Expanded} ({GetItemPrSecond(planner.Expanded, watch.Elapsed)}/s). Generated {planner.Generated} ({GetItemPrSecond(planner.Generated, watch.Elapsed)}/s). Evaluations {planner.Evaluations} ({GetItemPrSecond(planner.Evaluations, watch.Elapsed)}/s)", ConsoleColor.DarkGray);
            };
            logTimer.Start();
            watch.Start();

            WriteLineColor("\tStarting search...");
            if (opts.SearchTimeLimit > 0)
                planner.TimeLimit = TimeSpan.FromSeconds(opts.SearchTimeLimit);
            if (opts.SearchMemoryLimit > 0)
                planner.MemoryLimit = opts.SearchMemoryLimit;
            solution = planner.Solve(context);

            switch (planner.Code)
            {
                case ILimitedComponent.ReturnCode.TimedOut: WriteLineColor("\tPlanner timed out...", ConsoleColor.Yellow); return null;
                case ILimitedComponent.ReturnCode.MemoryLimitReached: WriteLineColor("\tPlanner reached memory limit...", ConsoleColor.Yellow); return null;
            }

            logTimer.Stop();
            watch.Stop();

            if (solution == null)
            {
                WriteLineColor("\tNo solution could be found!", ConsoleColor.Red);
            }
            else
            {
                WriteLineColor("\tSolution found!", ConsoleColor.Green);
                WriteLineColor($"\tSearch took {planner.ExecutionTime.TotalSeconds} seconds");
                WriteLineColor($"\tPeak memory usage: {planner.MemoryUsed}MB");
                WriteLineColor("\tPlanner info:");
                WriteLineColor($"\t\t{planner.Expanded} total expansions", ConsoleColor.DarkGray);
                WriteLineColor($"\t\t{planner.Generated} total generations", ConsoleColor.DarkGray);
                WriteLineColor($"\t\t{planner.Evaluations} total evaluations", ConsoleColor.DarkGray);

                var planGenerator = new FastDownwardPlanGenerator(new ErrorListener());
                var plan = planGenerator.Generate(solution);
                WriteLineColor($"Plan", ConsoleColor.Blue);
                WriteLineColor($"\tPlan has {solution.Plan.Count} steps with a cost of {solution.Cost}");
                WriteLineColor($"\tPlan uses {solution.Plan.DistinctBy(x => x.ActionName).Count()} actions out of {pddlDecl.Domain.Actions.Count}");
                if (opts.PrintPlan)
                {
                    WriteLineColor($"\tThe plan is:");
                    WriteLineColor($"\t\t{plan.Replace(Environment.NewLine, $"{Environment.NewLine}\t\t")}", ConsoleColor.DarkGray);
                }

                WriteColor("\tOutputting plan file...");
                File.WriteAllText(opts.PlanPath, plan);
                WriteLineColor("Done!", ConsoleColor.Green);
            }

            return solution;
        }

        private static void Validate(ActionPlan plan, PDDLDecl decl)
        {
            WriteLineColor("Validating plan", ConsoleColor.Blue);
            WriteColor("\tValidating...");

            var validator = new PlanVal.PlanValidator();
            if (validator.Validate(plan, decl))
                WriteLineColor("Valid!", ConsoleColor.Green);
            else
            {
                WriteLineColor("Invalid!", ConsoleColor.Red);
                WriteLineColor($"\tReason: {validator.ValidationError}");
            }
        }

        private static void OnLog(string text) => WriteLineColor($"\t\t{text}", ConsoleColor.DarkGray);

        private static double GetItemPrSecond(int amount, TimeSpan elapsed)
        {
            if (elapsed.TotalMilliseconds == 0)
                return 0;
            return Math.Round(amount / (elapsed.TotalMilliseconds / 1000), 1);
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

        private static void WriteLineColor(string text, ConsoleColor? color = null)
        {
            if (color != null)
                Console.ForegroundColor = (ConsoleColor)color;
            else
                Console.ResetColor();
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void WriteColor(string text, ConsoleColor? color = null)
        {
            if (color != null)
                Console.ForegroundColor = (ConsoleColor)color;
            else
                Console.ResetColor();
            Console.Write(text);
            Console.ResetColor();
        }
    }
}