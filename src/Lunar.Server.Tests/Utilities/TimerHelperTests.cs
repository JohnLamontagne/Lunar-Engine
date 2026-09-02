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

            // Must not sleep past the requested period by more than a generous scheduler allowance,
            // and must not spin for the whole period on a platform without timer resolution APIs.
            Assert.InRange(sw.Elapsed.TotalMilliseconds, 0, 20 + 25);
        }
    }
}
