using Lidgren.Network;
using Lunar.Server.Content.Graphics;
using Lunar.Server.Net;
using Lunar.Server.World.Actors;
using System.Linq;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Server.Utilities;

namespace Lunar.Server.World.Structure
{
    class TorchMapObject : MapObject
    {
        public int PlayerDamage { get; set; }
        public int EffectCooldown { get; set; }

        public TorchMapObject(Sprite sprite, Layer layer)
            : base(layer)
        {
            this.PlayerDamage = 10;
            this.EffectCooldown = 2000;
            this.Sprite = sprite;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var player in this.InteractingEntities.OfType<Player>())
            {
                if (this.Cooldowns[player] <= gameTime.TotalElapsedTime)
                {
                    // Process special tile functionality.

                    player.SendChatMessage("Ouch -- that burns!", ChatMessageType.Alert);
                    player.InflictDamage(this.PlayerDamage);

                    var playSoundPacket = new Packet(PacketType.PLAY_SOUND);
                    playSoundPacket.Message.Write("torchburn");
                    playSoundPacket.Message.Write(100f);
                    player.SendPacket(playSoundPacket, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);

                    this.Cooldowns[player] = gameTime.TotalElapsedTime + this.EffectCooldown;
                }
                else if (this.Cooldowns[player] <= 0)
                {
                    this.Cooldowns[player] = gameTime.TotalElapsedTime + this.EffectCooldown;
                }
            }
        }
    }
}
