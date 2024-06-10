using System.Diagnostics;

namespace FlashPlanner.Core.Helpers
{
    internal static class MemoryHelper
    {
        internal static int GetMemoryUsageMB() => (int)(Process.GetCurrentProcess().PrivateMemorySize64 / 1000000);
    }
}
