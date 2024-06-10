using BenchmarkTool;
using FlashPlanner.Core;
using FlashPlanner.Core.Heuristics;
using FlashPlanner.Core.Search;
using FlashPlanner.Core.Translators;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.PDDL;
using ToMarkdown;

var tmpFolder = "tmp";
var targetBenchmarkFolder = "../../../../Dependencies/downward-benchmarks/";
var targetDomains = new List<string>() {
    "gripper",
    "miconic",
    "depot",
    "rovers",
    "zenotravel",
    "tpp",
    "satellite",
    "driverlog",
    "blocks",
    "logistics00",
    "logistics98",
    "freecell",
    "movie",
    "mprime",
    "trucks",
    "visitall-opt11-strips",
};
// Cached fast downward times
var fdTimes = new Dictionary<string, int>()
{
    {"gripper", 20 },
    {"miconic", 20 },
    {"depot", 15 },
    {"rovers", 20 },
    {"zenotravel", 20 },
    {"tpp", 20 },
    {"satellite", 20 },
    {"driverlog", 20 },
    {"blocks", 20 },
    {"logistics00", 20 },
    {"logistics98", 20 },
    {"freecell", 20 },
    {"movie", 20 },
    {"mprime", 20 },
    {"trucks", 14 },
    {"visitall-opt11-strips", 20 },
};
var problemsToUse = 20;
var timeLimit = 60;
var memoryLimit = 4096;

if (Directory.Exists(tmpFolder))
    Directory.Delete(tmpFolder, true);
Directory.CreateDirectory(tmpFolder);

var results = new List<CoverageResult>();

var listener = new ErrorListener();
var parser = new PDDLParser(listener);
var codeGenerator = new PDDLCodeGenerator(listener);

foreach (var domainName in targetDomains)
{
    Console.WriteLine($"Domain: {domainName}");
    var domainFile = Path.Combine(targetBenchmarkFolder, domainName, "domain.pddl");
    var domain = parser.ParseAs<DomainDecl>(new FileInfo(domainFile));
    var files = new DirectoryInfo(Path.Combine(targetBenchmarkFolder, domainName)).GetFiles().ToList();
    files.RemoveAll(x => x.Name == "domain.pddl" || x.Extension != ".pddl");
    files = files.Take(problemsToUse).ToList();

    //var solvedA = 0;
    var solvedB = 0;

    int count = 1;
    foreach (var file in files)
    {
        Console.WriteLine($"\tProblem {count++} out of {files.Count}");
        if (file.Name == "domain.pddl")
            continue;

        var problem = parser.ParseAs<ProblemDecl>(file);

        //codeGenerator.Generate(domain, Path.Combine(tmpFolder, "domain.pddl"));
        //codeGenerator.Generate(problem, Path.Combine(tmpFolder, "problem.pddl"));

        //if (File.Exists(Path.Combine(tmpFolder, "plan")))
        //    File.Delete(Path.Combine(tmpFolder, "plan"));

        //var fdProcess = new Process()
        //{
        //    StartInfo = new ProcessStartInfo()
        //    {
        //        FileName = "python3",
        //        Arguments = $"../../../../../Dependencies/downward/fast-downward.py --translate-time-limit {timeLimit} --search-time-limit {timeLimit} --plan-file plan domain.pddl problem.pddl --evaluator \"hff=ff()\" --search \"lazy_greedy([hff], preferred=[hff])\"",
        //        CreateNoWindow = true,
        //        UseShellExecute = false,
        //        WorkingDirectory = tmpFolder,
        //        RedirectStandardError = true,
        //        RedirectStandardOutput = true
        //    }
        //};

        //fdProcess.Start();
        //fdProcess.WaitForExit();

        //if (File.Exists(Path.Combine(tmpFolder, "plan")))
        //    solvedA++;

        var translator = new PDDLToSASTranslator(false);
        translator.TimeLimit = TimeSpan.FromSeconds(timeLimit);
        translator.MemoryLimit = memoryLimit;
        var sasDecl = translator.Translate(new PDDLSharp.Models.PDDL.PDDLDecl(domain, problem));
        var planner = new GreedyBFSLazy(new hFF());
        planner.TimeLimit = TimeSpan.FromSeconds(timeLimit);
        planner.MemoryLimit = memoryLimit;
        var plan = planner.Solve(sasDecl);
        if (planner.Code == ILimitedComponent.ReturnCode.Success && plan != null)
            solvedB++;
    }

    //results.Add(new CoverageResult(domainName, files.Count, solvedA, solvedB));
    results.Add(new CoverageResult(domainName, files.Count, fdTimes[domainName], solvedB));
}

results.Add(new CoverageResult("Total", results.Sum(x => x.Problems), results.Sum(x => x.SolvedA), results.Sum(x => x.SolvedB)));

var targetReadme = "../../../../readme.md";
var allText = File.ReadAllText(targetReadme);
allText = allText.Substring(0, allText.IndexOf("<!-- This section is auto generated. -->") + "<!-- This section is auto generated. -->".Length);
allText += Environment.NewLine;
allText += results.ToMarkdownTable(new List<string>() { "Domain", "Problems", "Fast Downward", "Flash Planner" });

File.WriteAllText(targetReadme, allText);