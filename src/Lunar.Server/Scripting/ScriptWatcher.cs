using System;
using System.IO;
using System.Threading;

namespace Lunar.Server.Scripting
{
    internal sealed class ScriptWatcher : IDisposable
    {
        private readonly FileSystemWatcher _fsw;
        private readonly Action _onChanged;
        private readonly Timer _debounce;
        private const int DebounceMs = 250;

        public ScriptWatcher(string root, Action onChanged)
        {
            _onChanged = onChanged;
            _debounce = new Timer(_ => _onChanged(), null, Timeout.Infinite, Timeout.Infinite);

            _fsw = new FileSystemWatcher(root, "*.cs")
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size,
            };
            _fsw.Changed += Bump;
            _fsw.Created += Bump;
            _fsw.Deleted += Bump;
            _fsw.Renamed += Bump;
            _fsw.EnableRaisingEvents = true;
        }

        private void Bump(object sender, FileSystemEventArgs e)
            => _debounce.Change(DebounceMs, Timeout.Infinite);

        public void Dispose()
        {
            _fsw.Dispose();
            _debounce.Dispose();
        }
    }
}
