﻿using BenchmarkTool;
using FlashPlanner.Heuristics;
using FlashPlanner.Search.Classical;
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
    "satellite",
    "miconic"
};

if (!Directory.Exists(tmpFolder))
    Directory.CreateDirectory(tmpFolder);

var timeLimit = 60;
var memoryLimit = 4096;

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
    files = files.Take(20).ToList();

    var solvedA = 0;
    var solvedB = 0;

    int count = 1;
    foreach (var file in files)
    {
        Console.WriteLine($"\tProblem {count++} out of {files.Count}");
        if (file.Name == "domain.pddl")
            continue;

        var problem = parser.ParseAs<ProblemDecl>(file);

        codeGenerator.Generate(domain, Path.Combine(tmpFolder, "domain.pddl"));
        codeGenerator.Generate(problem, Path.Combine(tmpFolder, "problem.pddl"));

        if (File.Exists(Path.Combine(tmpFolder, "plan")))
            File.Delete(Path.Combine(tmpFolder, "plan"));

        var fdProcess = new Process() 
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "python3",
                Arguments = $"../../../../../Dependencies/downward/fast-downward.py --translate-time-limit {timeLimit} --search-time-limit {timeLimit} --plan-file plan domain.pddl problem.pddl --evaluator \"hff=ff()\" --search \"lazy_greedy([hff], preferred=[hff])\"",
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = tmpFolder,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            }
        };

        fdProcess.Start();
        fdProcess.WaitForExit();

        if (File.Exists(Path.Combine(tmpFolder, "plan")))
            solvedA++;

        var translator = new PDDLToSASTranslator(true);
        translator.TimeLimit = TimeSpan.FromSeconds(timeLimit);
        translator.MemoryLimit = memoryLimit;
        var sasDecl = translator.Translate(new PDDLSharp.Models.PDDL.PDDLDecl(domain, problem));
        using (var planner = new GreedyBFS(sasDecl, new hFF(sasDecl)))
        {
            planner.TimeLimit = TimeSpan.FromSeconds(timeLimit);
            planner.MemoryLimit = memoryLimit;
            var plan = planner.Solve();
            if (planner.Code == FlashPlanner.ILimitedComponent.ReturnCode.Success && plan.Plan.Count > 0)
                solvedB++;
        }
    }

    results.Add(new CoverageResult(domainName, files.Count, solvedA, solvedB));
}

var targetReadme = "../../../../readme.md";
var allText = File.ReadAllText(targetReadme);
allText = allText.Substring(0, allText.IndexOf("<!-- This section is auto generated. -->") + "<!-- This section is auto generated. -->".Length);
allText += Environment.NewLine;
allText += results.ToMarkdownTable(new List<string>() { "Domain", "Problems", "Fast Downward", "Flash Planner" });

File.WriteAllText(targetReadme, allText);