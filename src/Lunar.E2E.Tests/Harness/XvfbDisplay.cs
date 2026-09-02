using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Lunar.E2E.Tests.Harness
{
    /// <summary>
    /// Starts a private Xvfb display on Linux so the client can create a GL context on a machine with
    /// no GPU or desktop. On other platforms it is a no-op and the client uses the real display.
    /// </summary>
    public sealed class XvfbDisplay : IDisposable
    {
        private ManagedProcess _xvfb;

        public string Display { get; private set; }
        public bool IsActive => _xvfb != null;

        public static bool IsSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public XvfbDisplay(int width, int height, string logPath)
        {
            if (!IsSupported)
                return;

            // Pick a display number whose socket does not already exist.
            int number = 90 + Random.Shared.Next(0, 800);
            while (File.Exists($"/tmp/.X11-unix/X{number}"))
                number++;

            Display = $":{number}";
            _xvfb = new ManagedProcess("Xvfb", "Xvfb",
                new[] { Display, "-screen", "0", $"{width}x{height}x24", "-nolisten", "tcp" },
                Environment.CurrentDirectory,
                new Dictionary<string, string>(),
                logPath);

            var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            while (!File.Exists($"/tmp/.X11-unix/X{number}"))
            {
                if (_xvfb.HasExited)
                    throw new InvalidOperationException("Xvfb exited immediately:\n" + _xvfb.Output);
                if (DateTime.UtcNow > deadline)
                    throw new TimeoutException("Xvfb did not create its display socket in time.\n" + _xvfb.Output);
                Thread.Sleep(50);
            }
        }

        /// <summary>Environment variables a child process needs to render on this display.</summary>
        public void ApplyTo(IDictionary<string, string> environment)
        {
            if (!IsActive) return;
            environment["DISPLAY"] = Display;
            environment["SDL_VIDEODRIVER"] = "x11";
            environment["LIBGL_ALWAYS_SOFTWARE"] = "1";
            environment["SDL_AUDIODRIVER"] = "dummy";
        }

        public void Dispose() => _xvfb?.Dispose();
    }
}
