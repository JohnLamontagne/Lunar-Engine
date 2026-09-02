using System.Diagnostics;
using Lunar.Server.Utilities;
using Xunit;

namespace Lunar.Server.Tests.Utilities
{
    public class TimerHelperTests
    {
        [Fact]
        public void Resolution_is_a_sane_positive_value_on_every_platform()
        {
            double resolution = TimerHelper.GetCurrentResolution();
            Assert.InRange(resolution, 0.01, 50.0);
        }

        [Fact]
        public void SleepForNoMoreThan_never_overshoots_by_more_than_the_scheduler_slop()
        {
            var sw = Stopwatch.StartNew();
            TimerHelper.SleepForNoMoreThan(20);
            sw.Stop();

            // This guards against hanging or spinning far past the requested period on platforms without
            // timer-resolution APIs. Exact overshoot depends on the scheduler and the load on the test
            // machine, so the bound is deliberately generous rather than tight.
            Assert.InRange(sw.Elapsed.TotalMilliseconds, 0, 250);
        }
    }
}
