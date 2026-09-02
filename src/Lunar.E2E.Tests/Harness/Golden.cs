using System;
using System.IO;
using Xunit;

namespace Lunar.E2E.Tests.Harness
{
    /// <summary>
    /// Golden-image comparison. Goldens live under Lunar.E2E.Tests/Goldens and are committed.
    /// Set LUNAR_UPDATE_GOLDENS=1 to (re)write them from the current run instead of asserting.
    /// </summary>
    public static class Golden
    {
        public static string Directory => Path.Combine(Paths.SourceRoot, "Lunar.E2E.Tests", "Goldens");

        public static bool UpdateMode => Environment.GetEnvironmentVariable("LUNAR_UPDATE_GOLDENS") == "1";

        /// <summary>
        /// Asserts <paramref name="actual"/> is within <paramref name="maxMeanDifference"/> of the named golden.
        /// When no golden exists yet the frame is written and the test passes, so a first run seeds them.
        /// </summary>
        public static void AssertMatches(string name, Frame actual, double maxMeanDifference, string artifactDir)
        {
            System.IO.Directory.CreateDirectory(Directory);
            string goldenPath = Path.Combine(Directory, name + ".png");

            if (UpdateMode || !File.Exists(goldenPath))
            {
                actual.Save(goldenPath);
                return;
            }

            using var golden = Frame.Load(goldenPath);
            double diff = actual.MeanAbsoluteDifference(golden);
            if (diff > maxMeanDifference)
            {
                actual.Save(Path.Combine(artifactDir, name + ".actual.png"));
                golden.Save(Path.Combine(artifactDir, name + ".expected.png"));
            }
            Assert.True(diff <= maxMeanDifference,
                $"Frame '{name}' differs from golden by mean {diff:F2} (limit {maxMeanDifference}). See {artifactDir}.");
        }
    }
}
