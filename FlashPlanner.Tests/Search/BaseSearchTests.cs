using FlashPlanner.Core.Heuristics;
using FlashPlanner.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Tests.Search
{
    public abstract class BaseSearchTests : BasePlannerTests
    {
        public static string DataPath = "../../../../Dependencies/downward-benchmarks/";
        public static List<string> TargetDomain = new List<string>() {
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
        public static int FirstNProblems = 1;
        public static List<Func<IHeuristic>> Heuristics = new List<Func<IHeuristic>>()
        {
            { () => new hGoal() },
            { () => new hAdd() },
            { () => new hMax() },
            { () => new hFF() },
        };

        public static IEnumerable<object[]> GetTestingData(List<string> domains = null)
        {
            if (domains == null)
                domains = TargetDomain;

            foreach (var fHeuristic in Heuristics)
            {
                foreach (var domainName in domains)
                {
                    var targetFolder = new DirectoryInfo(Path.Combine(DataPath, domainName));
                    var domain = new FileInfo(Path.Combine(DataPath, domainName, "domain.pddl"));
                    var problems = new List<FileInfo>();
                    foreach (var problem in targetFolder.GetFiles().OrderBy(x => x.Name.Length).ThenBy(x => x.Name))
                    {
                        if (problem.Name != "domain.pddl")
                            problems.Add(problem);
                        if (problems.Count >= FirstNProblems)
                            break;
                    }
                    foreach (var problem in problems)
                        yield return new object[] { domain.FullName, problem.FullName, fHeuristic() };
                }
            }
        }
    }
}
