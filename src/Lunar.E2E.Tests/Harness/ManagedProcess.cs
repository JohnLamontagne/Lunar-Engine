using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lunar.E2E.Tests.Harness
{
    /// <summary>
    /// A child process whose stdout/stderr are captured to memory and to a log file, with a helper to
    /// wait for a marker line. Killed (with its process tree) on dispose.
    /// </summary>
    public sealed class ManagedProcess : IDisposable
    {
        private readonly Process _process;
        private readonly StringBuilder _output = new StringBuilder();
        private readonly List<TaskCompletionSource<string>> _lineWaiters = new List<TaskCompletionSource<string>>();
        private readonly List<Func<string, bool>> _linePredicates = new List<Func<string, bool>>();
        private readonly StreamWriter _log;
        private readonly object _gate = new object();

        public string Name { get; }
        public bool HasExited => _process.HasExited;
        public int ExitCode => _process.ExitCode;
        public string Output { get { lock (_gate) return _output.ToString(); } }

        public ManagedProcess(string name, string fileName, IEnumerable<string> arguments, string workingDirectory,
                              IDictionary<string, string> environment, string logPath)
        {
            Name = name;
            _log = new StreamWriter(logPath, false, Encoding.UTF8) { AutoFlush = true };

            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            foreach (var arg in arguments)
                psi.ArgumentList.Add(arg);
            foreach (var kv in environment)
                psi.Environment[kv.Key] = kv.Value;

            _process = new Process { StartInfo = psi, EnableRaisingEvents = true };
            _process.OutputDataReceived += (_, e) => OnLine(e.Data);
            _process.ErrorDataReceived += (_, e) => OnLine(e.Data);
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        private void OnLine(string line)
        {
            if (line == null) return;
            lock (_gate)
            {
                _output.AppendLine(line);
                _log.WriteLine(line);
                for (int i = _linePredicates.Count - 1; i >= 0; i--)
                {
                    if (_linePredicates[i](line))
                    {
                        _lineWaiters[i].TrySetResult(line);
                        _linePredicates.RemoveAt(i);
                        _lineWaiters.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>Waits for a line matching <paramref name="predicate"/>, checking lines already received.</summary>
        public async Task<string> WaitForLineAsync(Func<string, bool> predicate, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (_gate)
            {
                foreach (var existing in _output.ToString().Split('\n'))
                    if (predicate(existing.TrimEnd('\r')))
                        return existing;
                _linePredicates.Add(predicate);
                _lineWaiters.Add(tcs);
            }

            var exited = Task.Run(() => _process.WaitForExit());
            var completed = await Task.WhenAny(tcs.Task, exited, Task.Delay(timeout));
            if (completed == tcs.Task)
                return await tcs.Task;
            if (completed == exited)
                throw new InvalidOperationException($"{Name} exited with code {_process.ExitCode} before the expected output appeared.\n--- {Name} output ---\n{Output}");
            throw new TimeoutException($"{Name} did not produce the expected output within {timeout}.\n--- {Name} output ---\n{Output}");
        }

        public int Id => _process.Id;

        /// <summary>Waits up to <paramref name="timeout"/> for the process to exit on its own.</summary>
        public bool WaitForExit(TimeSpan timeout) => _process.HasExited || _process.WaitForExit((int)timeout.TotalMilliseconds);

        /// <summary>Sends SIGTERM (Linux/macOS) and waits for exit; returns the exit code. Falls back to Kill on Windows.</summary>
        public async Task<int> StopGracefullyAsync(TimeSpan timeout)
        {
            if (_process.HasExited)
                return _process.ExitCode;

            if (OperatingSystem.IsWindows())
            {
                _process.Kill(entireProcessTree: false);
            }
            else
            {
                using var kill = Process.Start(new ProcessStartInfo("kill", $"-TERM {_process.Id}") { UseShellExecute = false });
                kill.WaitForExit();
            }

            var exited = Task.Run(() => _process.WaitForExit((int)timeout.TotalMilliseconds));
            if (!await exited)
                throw new TimeoutException($"{Name} did not exit within {timeout} after SIGTERM.\n--- {Name} output ---\n{Output}");
            return _process.ExitCode;
        }

        public void Dispose()
        {
            try
            {
                if (!_process.HasExited)
                {
                    _process.Kill(entireProcessTree: true);
                    _process.WaitForExit(5000);
                }
            }
            catch { /* already gone */ }
            _process.Dispose();
            _log.Dispose();
        }
    }
}
