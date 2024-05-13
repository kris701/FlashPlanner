using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashPlanner.Helpers
{
    internal static class MemoryHelper
    {
        internal static int GetMemoryUsageMB() => (int)(Process.GetCurrentProcess().PrivateMemorySize64 / 1000000);
    }
}
