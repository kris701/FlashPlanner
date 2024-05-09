using FlashPlanner.Heuristics;
using FlashPlanner.HeuristicsCollections;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;

namespace FlashPlanner.CLI
{
    public static class HeuristicBuilder
    {
        public static IHeuristic ParseHeuristic(string text, SASDecl sasDecl, PDDLDecl pddlDecl)
        {
            var subText = text.Substring(text.IndexOf('(') + 1, text.LastIndexOf(')') - text.IndexOf('(') - 1);
            if (text.StartsWith("hColMax"))
            {
                var subH = new List<IHeuristic>();
                var all = subText.Split(',');
                foreach (var item in all)
                    if (item != "")
                        subH.Add(ParseHeuristic(item, sasDecl, pddlDecl));
                return new hColMax(subH);
            }
            if (text.StartsWith("hColSum"))
            {
                var subH = new List<IHeuristic>();
                var all = subText.Split(',');
                foreach (var item in all)
                    if (item != "")
                        subH.Add(ParseHeuristic(item, sasDecl, pddlDecl));
                return new hColMax(subH);
            }

            if (text.StartsWith("hAdd"))
                return new hAdd();
            if (text.StartsWith("hConstant"))
                return new hConstant(int.Parse(subText));
            if (text.StartsWith("hDepth"))
                return new hDepth();
            if (text.StartsWith("hFF"))
                return new hFF(sasDecl);
            if (text.StartsWith("hGoal"))
                return new hGoal();
            if (text.StartsWith("hMax"))
                return new hMax();
            if (text.StartsWith("hPath"))
                return new hPath();
            if (text.StartsWith("hWeighted"))
            {
                var split = subText.Split(',');
                return new hWeighted(ParseHeuristic(split[0], sasDecl, pddlDecl), double.Parse(split[1]));
            }

            throw new Exception("Unknown heuristic!");
        }
    }
}
