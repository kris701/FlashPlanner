using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FlashPlanner.LimitedComponent;

namespace FlashPlanner
{
    public interface ILimitedComponent
    {
        public enum ReturnCode { None, Success, TimedOut, MemoryLimitReached }

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
