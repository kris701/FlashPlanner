using BenchmarkTool;
using FlashPlanner.Heuristics;
using FlashPlanner.Search;
using FlashPlanner.Translators;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.PDDL;
using System.Diagnostics;
using ToMarkdown;

var tmpFolder = "tmp";
var targetBenchmarkFolder = "../../../../Dependencies/downward-benchmarks/";
var targetDomains = new List<string>() {
    "blocks",
    "depot",
    "gripper",
    "logistics00",
    "satellite",
    "miconic",
    "mystery",
    "rovers",
    "tpp",
    "zenotravel",
};
// Cached fast downward times
var fdTimes = new Dictionary<string, int>()
{
    { "blocks", 20 },
    { "depot", 15 },
    { "gripper", 20 },
    { "logistics00", 20 },
    { "satellite", 20 },
    { "miconic", 20 },
    { "mystery", 12 },
    { "rovers", 20 },
    { "tpp", 20 },
    { "zenotravel", 20 },
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

        var translator = new PDDLToSASTranslator(true, false);
        translator.TimeLimit = TimeSpan.FromSeconds(timeLimit);
        translator.MemoryLimit = memoryLimit;
        var sasDecl = translator.Translate(new PDDLSharp.Models.PDDL.PDDLDecl(domain, problem));
        var planner = new GreedyBFS(new hFF());
        planner.TimeLimit = TimeSpan.FromSeconds(timeLimit);
        planner.MemoryLimit = memoryLimit;
        var plan = planner.Solve(sasDecl);
        if (planner.Code == FlashPlanner.ILimitedComponent.ReturnCode.Success && plan != null)
            solvedB++;
    }

    results.Add(new CoverageResult(domainName, files.Count, fdTimes[domainName], solvedB));
}

var targetReadme = "../../../../readme.md";
var allText = File.ReadAllText(targetReadme);
allText = allText.Substring(0, allText.IndexOf("<!-- This section is auto generated. -->") + "<!-- This section is auto generated. -->".Length);
allText += Environment.NewLine;
allText += results.ToMarkdownTable(new List<string>() { "Domain", "Problems", "Fast Downward", "Flash Planner" });

File.WriteAllText(targetReadme, allText);