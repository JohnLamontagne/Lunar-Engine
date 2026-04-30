using Lunar.Server.World.Actors;
using Lunar.Server.World.Conversation;

namespace Lunar.Server.Scripting.Api
{
    /// <summary>
    /// Base class for a dialogue script. <c>Function</c> and <c>Condition</c>
    /// names on <see cref="DialogueResponse"/> are resolved to public methods on
    /// the concrete subclass via reflection.
    ///
    /// Function methods: <c>void Name(Dialogue dialogue, Player listener)</c>
    /// Condition methods: <c>bool Name(Dialogue dialogue, Player listener)</c>
    /// </summary>
    public abstract class DialogueScript
    {
    }
}
