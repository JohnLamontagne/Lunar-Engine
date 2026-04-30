using System;

namespace Lunar.Server.Scripting.Api
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class NpcBehaviorAttribute : Attribute
    {
        public string Key { get; }
        public NpcBehaviorAttribute(string key) => Key = key;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ItemBehaviorAttribute : Attribute
    {
        public string Key { get; }
        public ItemBehaviorAttribute(string key) => Key = key;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PlayerBehaviorAttribute : Attribute
    {
        public string Role { get; init; }
        public string Class { get; init; }
        public PlayerBehaviorAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DialogueScriptAttribute : Attribute
    {
        public string DialogueName { get; }
        public DialogueScriptAttribute(string dialogueName) => DialogueName = dialogueName;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CommandScriptAttribute : Attribute
    {
    }
}
