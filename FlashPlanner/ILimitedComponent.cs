namespace FlashPlanner
{
    public delegate void LogEventHandler(string text);

    public interface ILimitedComponent
    {
        public enum ReturnCode { None, Success, TimedOut, MemoryLimitReached }

        public event LogEventHandler? DoLog;

        /// <summary>
        /// Return code from the component
        /// </summary>
        public ReturnCode Code { get; }
        /// <summary>
        /// Peak memory used by the component (in MB)
        /// </summary>
        public int MemoryUsed { get; }
        /// <summary>
        /// Memory limit for the component
        /// </summary>
        public int MemoryLimit { get; set; }
        /// <summary>
        /// Execution time
        /// </summary>
        public TimeSpan ExecutionTime { get; }
        /// <summary>
        /// Time limit for the component
        /// </summary>
        public TimeSpan TimeLimit { get; set; }
        /// <summary>
        /// Bool to stop execution
        /// </summary>
        public bool Abort { get; set; }
    }
}
