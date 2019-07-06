/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Client.Net;
using Lunar.Client.Utilities;
using Lunar.Client.World.Actors;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Core.World.Actor;

namespace Lunar.Client.World
{
    public class WorldManager : IService
    {
        private Map _map;

        private Player _player;

        private ContentManager _contentManager;

        private Camera _camera;

        private bool _mapLoaded;

        public Map Map => _map;

        /// <summary>
        /// The client's player.
        /// </summary>
        public Player Player => _player;

        public event EventHandler<PlayerJoinedEventArgs> PlayerJoined;

        public event EventHandler LoadingMap;

        public event EventHandler LoadedMap;

        public WorldManager(ContentManager contentManager, Camera camera)
        {
            _contentManager = contentManager;
            _camera = camera;

            Client.ServiceLocator.Get<NetHandler>().AddPacketHandler(PacketType.MAP_DATA, this.Handle_MapData);
            Client.ServiceLocator.Get<NetHandler>().AddPacketHandler(PacketType.PLAYER_JOINED, this.Handle_PlayerJoined);
            Client.ServiceLocator.Get<NetHandler>().AddPacketHandler(PacketType.PLAYER_LEFT, this.Handle_PlayerLeft);
            Client.ServiceLocator.Get<NetHandler>().AddPacketHandler(PacketType.PLAYER_DATA, this.Handle_PlayerData);
            Client.ServiceLocator.Get<NetHandler>().AddPacketHandler(PacketType.NPC_DATA, this.Handle_NPCData);
            Client.ServiceLocator.Get<NetHandler>().AddPacketHandler(PacketType.POSITION_UPDATE, this.Handle_PositionUpdate);
        }

        private void Handle_PositionUpdate(PacketReceivedEventArgs args)
        {
            long unqiueID = args.Message.ReadInt64();
            var player = _map.GetEntity<Player>(unqiueID);

            if (player != null)
            {
                string layerName = args.Message.ReadString();
                player.Layer = _map.GetLayer(layerName);
                player.Position = new Vector2(args.Message.ReadFloat(), args.Message.ReadFloat());
            }
        }

        private void Handle_PlayerLeft(PacketReceivedEventArgs args)
        {
            long uniqueID = args.Message.ReadInt64();
            _map.RemoveEntity(uniqueID);
        }

        private void Handle_NPCData(PacketReceivedEventArgs args)
        {
            var uniqueID = args.Message.ReadInt64();

            if (!_map.EntityExists(uniqueID))
            {
                var npc = new NPC(uniqueID);
                npc.Unpack(args.Message, _contentManager);
                _map.AddEntity(uniqueID, npc);
            }
            else
            {
                _map.GetEntity<NPC>(uniqueID).Unpack(args.Message, _contentManager);
            }
        }

        private void Handle_PlayerData(PacketReceivedEventArgs args)
        {
            long uniqueID = args.Message.ReadInt64();

            if (uniqueID == Client.ServiceLocator.Get<NetHandler>().UniqueID)
            {
                _player.Unpack(args.Message, _contentManager);
            }
            else
            {
                Player player = _map?.GetEntity<Player>(uniqueID);
                if (player != null)
                {
                    player.Unpack(args.Message, _contentManager);
                }
            }
        }

        private void Handle_PlayerJoined(PacketReceivedEventArgs args)
        {
            var uniqueID = args.Message.ReadInt64();

            Player player;

            if (uniqueID == Client.ServiceLocator.Get<NetHandler>().UniqueID)
            {
                if (_player == null)
                {
                    player = new Player(_camera, uniqueID);
                    player.Unpack(args.Message, _contentManager);
                    _player = player;
                }

                player = _player;
            }
            else
            {
                player = new Player(uniqueID);
                player.Unpack(args.Message, _contentManager);
            }

            _map?.AddEntity(uniqueID, player);

            this.PlayerJoined?.Invoke(this, new PlayerJoinedEventArgs(player));
        }

        private void Handle_MapData(PacketReceivedEventArgs args)
        {
            _mapLoaded = false;

            this.LoadingMap?.Invoke(this, new EventArgs());

            // Make sure the player is no longer moving
            if (_player != null)
                _player.State = ActorStates.Idle;

            var name = args.Message.ReadString();
            var dimensions = args.Message.ReadVector2();

            _map?.Unload(); // unload the previous map if it existed.

            _map = new Map(dimensions, name);
            _map.Unpack(args.Message);

            _mapLoaded = true;

            _camera.Bounds = new Rectangle(_map.Bounds.X * EngineConstants.TILE_SIZE, _map.Bounds.Y * EngineConstants.TILE_SIZE, _map.Bounds.Width * EngineConstants.TILE_SIZE, _map.Bounds.Height * EngineConstants.TILE_SIZE);

            this.LoadedMap?.Invoke(this, new EventArgs());
        }

        public void Update(GameTime gameTime)
        {
            if (_mapLoaded)
                _map?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_mapLoaded)
                _map?.Draw(spriteBatch, _camera);
        }

        public void RemovePlayer(long uniqueID)
        {
            _map.RemoveEntity(uniqueID);
        }

        public void Unload()
        {
            _map?.Unload();
            _map = null;
            _player = null;
        }

        public void Initalize()
        {
            throw new NotImplementedException();
        }
    }
}