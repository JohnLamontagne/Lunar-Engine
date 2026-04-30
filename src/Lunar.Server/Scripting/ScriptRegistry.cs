using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lunar.Server.Scripting.Api;

namespace Lunar.Server.Scripting
{
    public sealed class ScriptRegistry
    {
        public Assembly Assembly { get; }

        public IReadOnlyDictionary<string, Type> NpcBehaviors { get; }
        public IReadOnlyDictionary<string, Type> ItemBehaviors { get; }
        public IReadOnlyDictionary<string, Type> DialogueScripts { get; }
        public IReadOnlyList<Type> CommandScripts { get; }

        public Type DefaultPlayerBehavior { get; }
        public IReadOnlyDictionary<string, Type> PlayerBehaviorsByRole { get; }
        public IReadOnlyDictionary<string, Type> PlayerBehaviorsByClass { get; }

        public static ScriptRegistry Empty { get; } = new();

        private ScriptRegistry()
        {
            NpcBehaviors = new Dictionary<string, Type>();
            ItemBehaviors = new Dictionary<string, Type>();
            DialogueScripts = new Dictionary<string, Type>();
            CommandScripts = Array.Empty<Type>();
            PlayerBehaviorsByRole = new Dictionary<string, Type>();
            PlayerBehaviorsByClass = new Dictionary<string, Type>();
        }

        public ScriptRegistry(Assembly assembly)
        {
            Assembly = assembly;

            var npcs = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            var items = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            var dialogues = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            var commands = new List<Type>();
            var playersByRole = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            var playersByClass = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            Type defaultPlayer = null;

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || !type.IsClass) continue;

                if (type.GetCustomAttribute<NpcBehaviorAttribute>() is { } npcAttr
                    && typeof(NpcBehavior).IsAssignableFrom(type))
                    npcs[npcAttr.Key] = type;

                if (type.GetCustomAttribute<ItemBehaviorAttribute>() is { } itemAttr
                    && typeof(ItemBehavior).IsAssignableFrom(type))
                    items[itemAttr.Key] = type;

                if (type.GetCustomAttribute<DialogueScriptAttribute>() is { } dlgAttr
                    && typeof(DialogueScript).IsAssignableFrom(type))
                    dialogues[dlgAttr.DialogueName] = type;

                if (type.GetCustomAttribute<CommandScriptAttribute>() is not null
                    && typeof(CommandScript).IsAssignableFrom(type))
                    commands.Add(type);

                if (type.GetCustomAttribute<PlayerBehaviorAttribute>() is { } playerAttr
                    && typeof(PlayerBehavior).IsAssignableFrom(type))
                {
                    if (!string.IsNullOrEmpty(playerAttr.Class))
                        playersByClass[playerAttr.Class] = type;
                    else if (!string.IsNullOrEmpty(playerAttr.Role))
                        playersByRole[playerAttr.Role] = type;
                    else
                        defaultPlayer = type;
                }
            }

            NpcBehaviors = npcs;
            ItemBehaviors = items;
            DialogueScripts = dialogues;
            CommandScripts = commands;
            PlayerBehaviorsByRole = playersByRole;
            PlayerBehaviorsByClass = playersByClass;
            DefaultPlayerBehavior = defaultPlayer;
        }

        public Type ResolvePlayerBehavior(string role, string @class)
        {
            if (!string.IsNullOrEmpty(@class) && PlayerBehaviorsByClass.TryGetValue(@class, out var byClass))
                return byClass;
            if (!string.IsNullOrEmpty(role) && PlayerBehaviorsByRole.TryGetValue(role, out var byRole))
                return byRole;
            return DefaultPlayerBehavior;
        }
    }
}
