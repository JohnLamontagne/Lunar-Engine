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
using System.IO;
using System.Text.Json;
using Lunar.Core;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.FileSystem;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Actor;
using Lunar.Core.World.Actor.Descriptors;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class PlayerFSDataManager : FSDataManager<PlayerModel>
    {
        private record StatsDto(int Strength, int Intelligence, int Dexterity, int Defense, int Vitality);
        private record PlayerDto(
            string Name,
            string Password,
            string SpriteName,
            int SpriteFrameWidth,
            int SpriteFrameHeight,
            float Speed,
            StatsDto Stats,
            int Level,
            float PositionX,
            float PositionY,
            string MapID,
            string RoleName,
            int RoleLevel,
            float ReachX,
            float ReachY
        );

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public override PlayerModel Load(IDataManagerArguments arguments)
        {
            string filePath = this.RootPath + (arguments as PlayerDataArguments).Username + EngineConstants.ACC_FILE_EXT;

            try
            {
                string json = File.ReadAllText(filePath);
                var dto = JsonSerializer.Deserialize<PlayerDto>(json, JsonOptions);

                return new PlayerModel(dto.Name, dto.Password)
                {
                    SpriteSheet = new SpriteSheet(new SpriteInfo(dto.SpriteName), dto.SpriteFrameWidth, dto.SpriteFrameHeight),
                    Speed = dto.Speed,
                    Stats = new Stats()
                    {
                        Strength = dto.Stats.Strength,
                        Intelligence = dto.Stats.Intelligence,
                        Dexterity = dto.Stats.Dexterity,
                        Defense = dto.Stats.Defense,
                        Vitality = dto.Stats.Vitality,
                    },
                    Level = dto.Level,
                    Position = new Vector(dto.PositionX, dto.PositionY),
                    MapID = dto.MapID,
                    Role = new Role(dto.RoleName, dto.RoleLevel),
                    Reach = new Vector(dto.ReachX, dto.ReachY),
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override void Save(IContentModel descriptor, IDataManagerArguments arguments)
        {
            var p = (PlayerModel)descriptor;
            string filePath = this.RootPath + p.Name + EngineConstants.ACC_FILE_EXT;

            var dto = new PlayerDto(
                p.Name,
                p.Password,
                p.SpriteSheet.Sprite.TextureName,
                p.SpriteSheet.FrameWidth,
                p.SpriteSheet.FrameHeight,
                p.Speed,
                new StatsDto(p.Stats.Strength, p.Stats.Intelligence, p.Stats.Dexterity, p.Stats.Defense, p.Stats.Vitality),
                p.Level,
                p.Position.X,
                p.Position.Y,
                p.MapID,
                p.Role.Name,
                p.Role.Level,
                p.Reach.X,
                p.Reach.Y
            );

            File.WriteAllText(filePath, JsonSerializer.Serialize(dto, JsonOptions));
        }

        public override bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(this.RootPath + (arguments as PlayerDataArguments).Username + EngineConstants.ACC_FILE_EXT);
        }
    }
}
