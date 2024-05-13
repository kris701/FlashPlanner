using System.Diagnostics;

namespace FlashPlanner.Helpers
{
    internal static class MemoryHelper
    {
        internal static int GetMemoryUsageMB() => (int)(Process.GetCurrentProcess().PrivateMemorySize64 / 1000000);
    }
}
