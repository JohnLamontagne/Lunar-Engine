using Lunar.Core;
using Lunar.Server.Scripting.Api;
using Lunar.Server.World.Actors;

[PlayerBehavior]
public class DefaultPlayerBehavior : PlayerBehavior
{
    public override void OnCreated(IActor actor)
    {
    }

    public override void OnDeath(IActor actor)
    {
        var player = (Player)actor;
        player.Descriptor.Stats.Vitality = player.Descriptor.Stats.Vitality;
        player.JoinMap(player.Map);
        player.NetworkComponent.SendChatMessage("You have died!", ChatMessageType.Alert);
    }

    public override int Attack(IActor attacker, IActor target)
    {
        return 10;
    }

    public override void Attacked(IActor attacked, IActor attacker, int damageDelt)
    {
        var player = (Player)attacked;
        player.InflictDamage(damageDelt);
    }
}
