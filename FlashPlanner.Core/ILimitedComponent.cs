namespace FlashPlanner.Core
{
    /// <summary>
    /// Simple event to send text.
    /// </summary>
    /// <param name="text"></param>
    public delegate void LogEventHandler(string text);

    /// <summary>
    /// A container-like interface for making sure the process dont take too much time or memory.
    /// </summary>
    public interface ILimitedComponent
    {
        /// <summary>
        /// Possible return codes for the component
        /// </summary>
        public enum ReturnCode
        {
            /// <summary>
            /// Usually indicates some unknown error
            /// </summary>
            None,
            /// <summary>
            /// Indicates that the limiter didnt stop when it was running
            /// </summary>
            Success,
            /// <summary>
            /// Times out
            /// </summary>
            TimedOut,
            /// <summary>
            /// Hit the memory limit
            /// </summary>
            MemoryLimitReached
        }

        /// <summary>
        /// Logging event for the front end
        /// </summary>
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
