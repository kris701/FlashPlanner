namespace BenchmarkTool
{
    public class CoverageResult
    {
        public string Domain { get; set; }
        public int Problems { get; set; }
        public int SolvedA { get; set; }
        public int SolvedB { get; set; }

        public CoverageResult(string domain, int problems, int solvedA, int solvedB)
        {
            Domain = domain;
            Problems = problems;
            SolvedA = solvedA;
            SolvedB = solvedB;
        }
    }
}
