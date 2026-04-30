using Lunar.Core;
using Lunar.Core.Utilities.Data;
using Lunar.Server.Scripting.Api;

[CommandScript]
public class DefaultCommands : CommandScript
{
    public override void Register(ICommandRegistrar registrar)
    {
        registrar.Add("warpTo", ctx =>
        {
            if (!IsAdmin(ctx)) return;
            if (ctx.Args.Length < 2) return;
            if (!float.TryParse(ctx[0], out float x) || !float.TryParse(ctx[1], out float y)) return;

            ctx.Player.WarpTo(new Vector(x, y));
            ctx.Player.NetworkComponent.SendChatMessage($"Warped to {x}:{y}", ChatMessageType.Announcement);
        });

        registrar.Add("setSpeed", ctx =>
        {
            if (!IsAdmin(ctx)) return;
            if (ctx.Args.Length < 1) return;
            if (!float.TryParse(ctx[0], out float speed)) return;

            ctx.Player.Descriptor.Speed = speed;
            ctx.Player.NetworkComponent.SendChatMessage($"Speed set to {speed}", ChatMessageType.Announcement);
        });
    }

    private static bool IsAdmin(CommandContext ctx)
    {
        if (ctx.Player.Descriptor.Role == null || ctx.Player.Descriptor.Role.Level < 1)
        {
            ctx.Player.NetworkComponent.SendChatMessage("Permission denied.", ChatMessageType.Alert);
            return false;
        }
        return true;
    }
}
