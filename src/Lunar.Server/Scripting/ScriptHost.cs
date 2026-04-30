using System;
using System.IO;
using System.Threading;
using Lunar.Core.Utilities;
using Lunar.Server.Scripting.Api;

namespace Lunar.Server.Scripting
{
    /// <summary>
    /// Compiles every <c>.cs</c> file under <see cref="_scriptsRoot"/> into a
    /// single in-memory assembly hosted in a collectible <see cref="System.Runtime.Loader.AssemblyLoadContext"/>.
    /// On reload, the previous ALC is unloaded after the new generation has been
    /// installed so any references held to old script types must be replaced
    /// (subscribe to <see cref="ReloadCompleted"/>).
    /// </summary>
    public sealed class ScriptHost : IDisposable
    {
        private readonly string _scriptsRoot;
        private readonly Logger _logger;
        private readonly ScriptCompiler _compiler;
        private readonly object _swapLock = new();

        private Generation _current;
        private CollectibleScriptAlc _currentAlc;
        private ScriptWatcher _watcher;

        public event EventHandler ReloadCompleted;

        public ScriptHost(string scriptsRoot, Logger logger)
        {
            _scriptsRoot = scriptsRoot;
            _logger = logger;
            _compiler = new ScriptCompiler(scriptsRoot);
            _current = new Generation(ScriptRegistry.Empty);
        }

        public ScriptRegistry Registry => Volatile.Read(ref _current).Registry;

        public void Initialize()
        {
            Directory.CreateDirectory(_scriptsRoot);
            Reload();
            _watcher = new ScriptWatcher(_scriptsRoot, () => Reload());
        }

        public void Reload()
        {
            lock (_swapLock)
            {
                CompilationResult result;
                try
                {
                    result = _compiler.Compile();
                }
                catch (Exception ex)
                {
                    _logger.LogEvent($"Script compilation threw: {ex.Message}", LogTypes.ERROR, ex);
                    return;
                }

                if (result.IsEmpty)
                {
                    Volatile.Write(ref _current, new Generation(ScriptRegistry.Empty));
                    Console.WriteLine("Scripts: no .cs files found.");
                    ReloadCompleted?.Invoke(this, EventArgs.Empty);
                    return;
                }

                if (!result.Success)
                {
                    foreach (var err in result.Errors)
                        _logger.LogEvent("Script compile error: " + err, LogTypes.ERROR);
                    Console.WriteLine($"Scripts: compile failed with {result.Errors.Count} error(s); keeping previous generation.");
                    return;
                }

                var newAlc = new CollectibleScriptAlc("LunarScripts");
                var asm = newAlc.LoadFromStream(result.Assembly);
                var registry = new ScriptRegistry(asm);

                var previousAlc = _currentAlc;
                Volatile.Write(ref _current, new Generation(registry));
                _currentAlc = newAlc;

                Console.WriteLine($"Scripts: loaded {registry.NpcBehaviors.Count} npc, " +
                    $"{registry.ItemBehaviors.Count} item, {registry.DialogueScripts.Count} dialogue, " +
                    $"{registry.CommandScripts.Count} command, " +
                    $"{registry.PlayerBehaviorsByRole.Count + registry.PlayerBehaviorsByClass.Count + (registry.DefaultPlayerBehavior is null ? 0 : 1)} player behavior(s).");

                ReloadCompleted?.Invoke(this, EventArgs.Empty);

                if (previousAlc is not null)
                    UnloadInBackground(previousAlc);
            }
        }

        private void UnloadInBackground(CollectibleScriptAlc alc)
        {
            var weak = new WeakReference(alc);
            alc.Unload();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                for (int i = 0; i < 10 && weak.IsAlive; i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                if (weak.IsAlive)
                    _logger.LogEvent("Previous script ALC did not collect — something is still holding a reference to old script types.", LogTypes.GEN_SERVER);
            });
        }

        public bool TryCreateNpcBehavior(string key, out NpcBehavior behavior)
        {
            behavior = null;
            if (string.IsNullOrEmpty(key)) return false;
            if (!Registry.NpcBehaviors.TryGetValue(key, out var type)) return false;
            behavior = (NpcBehavior)Activator.CreateInstance(type);
            return true;
        }

        public bool TryCreateItemBehavior(string key, out ItemBehavior behavior)
        {
            behavior = null;
            if (string.IsNullOrEmpty(key)) return false;
            if (!Registry.ItemBehaviors.TryGetValue(key, out var type)) return false;
            behavior = (ItemBehavior)Activator.CreateInstance(type);
            return true;
        }

        public PlayerBehavior CreatePlayerBehavior(string role, string @class)
        {
            var type = Registry.ResolvePlayerBehavior(role, @class);
            return type is null ? null : (PlayerBehavior)Activator.CreateInstance(type);
        }

        public DialogueScript CreateDialogueScript(string dialogueName)
        {
            if (string.IsNullOrEmpty(dialogueName)) return null;
            if (!Registry.DialogueScripts.TryGetValue(dialogueName, out var type)) return null;
            return (DialogueScript)Activator.CreateInstance(type);
        }

        public void Dispose()
        {
            _watcher?.Dispose();
            _currentAlc?.Unload();
        }

        private sealed class Generation
        {
            public ScriptRegistry Registry { get; }
            public Generation(ScriptRegistry registry) => Registry = registry;
        }
    }
}
