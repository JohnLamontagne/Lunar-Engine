using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lunar.Server.Utilities
{
    // The following class (TimerHelper) is from: https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Utilities/TimerHelper.cs. License: https://github.com/MonoGame/MonoGame/blob/develop/LICENSE.txt
    internal static class TimerHelper
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtQueryTimerResolution(out uint MinimumResolution, out uint MaximumResolution, out uint CurrentResolution);

        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Assumed scheduler granularity on non-Windows hosts. Linux and macOS use high-resolution
        /// timers, so Thread.Sleep overshoots by roughly a millisecond in practice.
        /// </summary>
        private const double DefaultResolutionMilliseconds = 1.0;

        private static readonly double LowestSleepThreshold;

        static TimerHelper()
        {
            LowestSleepThreshold = 1.0 + QueryResolution();
        }

        private static double QueryResolution()
        {
            if (!IsWindows)
                return DefaultResolutionMilliseconds;

            try
            {
                NtQueryTimerResolution(out _, out uint max, out _);
                return max / 10000.0;
            }
            catch (DllNotFoundException)
            {
                return DefaultResolutionMilliseconds;
            }
            catch (EntryPointNotFoundException)
            {
                return DefaultResolutionMilliseconds;
            }
        }

        /// <summary>
        /// Returns the current timer resolution in milliseconds
        /// </summary>
        public static double GetCurrentResolution()
        {
            if (!IsWindows)
                return DefaultResolutionMilliseconds;

            NtQueryTimerResolution(out _, out _, out uint current);
            return current / 10000.0;
        }

        /// <summary>
        /// Sleeps as long as possible without exceeding the specified period
        /// </summary>
        public static void SleepForNoMoreThan(double milliseconds)
        {
            // Assumption is that Thread.Sleep(t) will sleep for at least (t), and at most (t + timerResolution)
            if (milliseconds < LowestSleepThreshold)
                return;
            var sleepTime = (int)(milliseconds - GetCurrentResolution());
            if (sleepTime > 0)
                Thread.Sleep(sleepTime);
        }
    }
}
