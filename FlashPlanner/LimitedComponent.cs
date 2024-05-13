using FlashPlanner.Helpers;
using FlashPlanner.Translators.Components;
using PDDLSharp.Translators.Grounders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FlashPlanner
{
    public abstract class LimitedComponent : ILimitedComponent
    {
        /// <summary>
        /// Return code from the component
        /// </summary>
        public ILimitedComponent.ReturnCode Code { get; internal set; }
        /// <summary>
        /// Peak memory used by the component (in MB)
        /// </summary>
        public int MemoryUsed { get; private set; }
        /// <summary>
        /// Memory limit for the component
        /// </summary>
        public int MemoryLimit { get; set; }
        /// <summary>
        /// Execution time
        /// </summary>
        public TimeSpan ExecutionTime { get; private set; }
        /// <summary>
        /// Time limit for the component
        /// </summary>
        public TimeSpan TimeLimit { get; set; }
        /// <summary>
        /// Bool to stop execution
        /// </summary>
        public bool Abort { get; set; }

        private System.Timers.Timer _timeoutTimer = new System.Timers.Timer();
        private System.Timers.Timer _memoryTimer = new System.Timers.Timer();
        private Stopwatch _executionTime = new Stopwatch();

        public void Start()
        {
            Code = ILimitedComponent.ReturnCode.None;

            if (TimeLimit > TimeSpan.Zero)
            {
                _timeoutTimer.Interval = TimeLimit.TotalMilliseconds;
                _timeoutTimer.AutoReset = false;
                _timeoutTimer.Elapsed += OnTimedOut;
                _timeoutTimer.Start();
            }
            if (MemoryLimit > 0)
            {
                _memoryTimer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
                _memoryTimer.AutoReset = true;
                _memoryTimer.Elapsed += CheckMemoryLimit;
                _memoryTimer.Start();
            }

            _executionTime.Start();
        }

        private void OnTimedOut(object? source, ElapsedEventArgs e)
        {
            Code = ILimitedComponent.ReturnCode.TimedOut;
            Stop();
            Abort = true;
            DoAbort();
        }

        private void CheckMemoryLimit(object? source, ElapsedEventArgs e)
        {
            if (MemoryHelper.GetMemoryUsageMB() > MemoryLimit)
            {
                Code = ILimitedComponent.ReturnCode.MemoryLimitReached;
                Stop();
                Abort = true;
                DoAbort();
            }
        }

        public void Stop()
        {
            _timeoutTimer.Stop();
            _memoryTimer.Stop();
            _executionTime.Stop();

            ExecutionTime = _executionTime.Elapsed;
            MemoryUsed = MemoryHelper.GetMemoryUsageMB();

            if (Code == ILimitedComponent.ReturnCode.None)
                Code = ILimitedComponent.ReturnCode.Success;
        }

        public virtual void DoAbort()
        {

        }
    }
}
