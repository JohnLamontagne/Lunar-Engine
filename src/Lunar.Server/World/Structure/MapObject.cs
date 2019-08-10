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

using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.Actors;
using System;
using System.Collections.Generic;
using System.IO;
using Lidgren.Network;
using Lunar.Core;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;
using Lunar.Server.Utilities;
using Lunar.Core.Net;
using Lunar.Core.World.Actor.Descriptors;

namespace Lunar.Server.World.Structure
{
    public class MapObject
    {
        private List<IActor<IActorDescriptor>> _interactingEntities;
        private Dictionary<IActor<IActorDescriptor>, double> _cooldowns;
        private bool _blocked;

        protected Dictionary<IActor<IActorDescriptor>, double> Cooldowns { get { return _cooldowns; } }
        protected List<IActor<IActorDescriptor>> InteractingEntities { get { return _interactingEntities; } }

        public Vector Position { get; set; }
        public SpriteInfo Sprite { get; set; }
        public bool Interactable { get; set; }
        public Layer Layer { get; set; }
        public CollisionBody CollisionDescriptor { get; set; }
        public bool Animated { get; set; }
        public int FrameTime { get; set; }
        public LightInformation LightInformation { get; set; }
        public MapObjectBehaviorDefinition MapObjectBehaviorDefinition { get; set; }

        public bool Blocked
        {
            get => _blocked;
            set
            {
                _blocked = value;
                _interactingEntities.Clear();
            }
        }

        public MapObject(Layer layer)
        {
            _interactingEntities = new List<IActor<IActorDescriptor>>();
            _cooldowns = new Dictionary<IActor<IActorDescriptor>, double>();

            this.MapObjectBehaviorDefinition = new MapObjectBehaviorDefinition();

            this.Layer = layer;
        }

        public virtual void OnLeft(IActor<IActorDescriptor> actor)
        {
            if (_interactingEntities.Contains(actor))
            {
                _interactingEntities.Remove(actor);
                _cooldowns.Remove(actor);
                this.MapObjectBehaviorDefinition?.OnLeft?.Invoke(this, actor);
            }
        }

        public virtual void OnEntered(IActor<IActorDescriptor> actor)
        {
            if (this.Blocked)
                return;

            if (actor.Layer == this.Layer)
            {
                if (!_interactingEntities.Contains(actor))
                {
                    _interactingEntities.Add(actor);
                    _cooldowns.Add(actor, 0);
                    this.MapObjectBehaviorDefinition?.OnEntered?.Invoke(this, actor);
                }
            }
        }

        public void OnInteract(IActor<IActorDescriptor> actor)
        {
            this.MapObjectBehaviorDefinition.OnInteract?.Invoke(this, actor);
        }

        public virtual void Update(GameTime gameTime)
        {
            this.MapObjectBehaviorDefinition?.Update?.Invoke(this, gameTime);
        }

        public NetBuffer Pack()
        {
            NetBuffer netBuffer = new NetBuffer();

            // Can we even see this mapObject?
            if (this.Sprite == null)
            {
                netBuffer.Write(false);
                return netBuffer;
            }

            netBuffer.Write(true); // we can see it
            netBuffer.Write(this.Sprite.TextureName);
            netBuffer.Write(this.Sprite.Transform.Rect);
            netBuffer.Write(this.Sprite.Transform.Color);
            netBuffer.Write(this.Position);
            netBuffer.Write(this.Animated);
            netBuffer.Write(this.Layer.LayerIndex);
            netBuffer.Write(this.FrameTime);

            // Is it a light?
            if (this.LightInformation == null)
            {
                netBuffer.Write(false);
            }
            else
            {
                netBuffer.Write(true);
                netBuffer.Write(this.LightInformation.Color);
                netBuffer.Write(this.LightInformation.Radius);
            }

            return netBuffer;
        }

        public static MapObject Load(BinaryReader bR, Layer layer)
        {
            var mapObject = new MapObject(layer)
            {
                Position = new Vector(bR.ReadSingle(), bR.ReadSingle())
            };

            if (bR.ReadBoolean())
            {
                string texturePath = bR.ReadString();
                mapObject.Sprite = new SpriteInfo(texturePath)
                {
                    Transform =
                    {
                        Rect = new Rect(bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32())
                    }
                };
            }

            mapObject.Animated = bR.ReadBoolean();

            mapObject.FrameTime = bR.ReadInt32();

            if (mapObject.Sprite != null)
            {
                mapObject.CollisionDescriptor = new CollisionBody(new Rect(
                    mapObject.Position.X, mapObject.Position.Y,
                    mapObject.Sprite.Transform.Rect.Width,
                    mapObject.Sprite.Transform.Rect.Height));
            }
            else
            {
                mapObject.CollisionDescriptor = new CollisionBody(new Rect(
                    mapObject.Position.X, mapObject.Position.Y,
                    Settings.TileSize, Settings.TileSize));
            }

            string scriptPath = bR.ReadString();
            if (!string.IsNullOrEmpty(scriptPath))
            {
                var script = Engine.Services.Get<ScriptManager>().CreateScript(Constants.FILEPATH_DATA + scriptPath);
                //mapObject.MapObjectBehaviorDefinition = script.Invoke<MapObjectBehaviorDefinition>("get_behavior_def", new ServerArgs(null));
            }

            var lightSource = bR.ReadBoolean();
            var lightRadius = bR.ReadSingle();
            var lightColor = new Color(bR.ReadByte(), bR.ReadByte(), bR.ReadByte(), bR.ReadByte());

            if (lightSource)
            {
                mapObject.LightInformation = new LightInformation()
                {
                    Radius = lightRadius,
                    Color = lightColor
                };
            }

            return mapObject;
        }
    }
}