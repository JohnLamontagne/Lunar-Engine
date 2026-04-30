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
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Structure;
using Lunar.Core.World.Structure.Attribute;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class MapFSDataManager : FSDataManager<MapModel<LayerModel<TileModel<SpriteInfo>>>>
    {
        private record VectorDto(float X, float Y);
        private record RectDto(int X, int Y, int Width, int Height);
        private record ColorDto(byte R, byte G, byte B, byte A);

        // Null SpriteName means no sprite on this tile; null AttributeData means no attribute.
        private record TileDto(
            string AttributeData,
            bool Animated,
            bool LightSource,
            string SpriteName,
            RectDto SpriteRect,
            ColorDto SpriteColor,
            float LayerDepth,
            int FrameCount
        );

        private record TileEntryDto(int X, int Y, TileDto Tile);

        private record LayerDto(string Name, int LayerIndex, List<TileEntryDto> Tiles);

        private record MapDto(
            List<string> TilesetPaths,
            string Name,
            VectorDto Dimensions,
            bool Dark,
            List<LayerDto> Layers
        );

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public override MapModel<LayerModel<TileModel<SpriteInfo>>> Load(IDataManagerArguments arguments)
        {
            var mapArguments = (arguments as ContentFileDataLoaderArguments);
            string json = File.ReadAllText(this.RootPath + mapArguments.FileName + EngineConstants.MAP_FILE_EXT);
            var dto = JsonSerializer.Deserialize<MapDto>(json, JsonOptions);

            var dimensions = new Vector(dto.Dimensions.X, dto.Dimensions.Y);
            var map = new MapModel<LayerModel<TileModel<SpriteInfo>>>(dimensions, dto.Name)
            {
                Dark = dto.Dark
            };
            map.TilesetPaths.AddRange(dto.TilesetPaths);
            map.Bounds = new Rect(0, 0, (int)dimensions.X, (int)dimensions.Y);

            foreach (var layerDto in dto.Layers)
            {
                var layer = new LayerModel<TileModel<SpriteInfo>>(dimensions, layerDto.Name, layerDto.LayerIndex);

                foreach (var entry in layerDto.Tiles)
                {
                    var tileDto = entry.Tile;
                    var tile = new TileModel<SpriteInfo>(new Vector(entry.X * EngineConstants.TILE_SIZE, entry.Y * EngineConstants.TILE_SIZE));

                    if (tileDto.AttributeData != null)
                        tile.Attribute = TileAttribute.Deserialize(Convert.FromBase64String(tileDto.AttributeData));

                    if (tileDto.SpriteName != null)
                    {
                        tile.Animated = tileDto.Animated;
                        tile.LightSource = tileDto.LightSource;
                        tile.Sprite = new SpriteInfo(tileDto.SpriteName)
                        {
                            Transform =
                            {
                                Position = new Vector(entry.X * EngineConstants.TILE_SIZE, entry.Y * EngineConstants.TILE_SIZE),
                                Color = new Color(tileDto.SpriteColor.R, tileDto.SpriteColor.G, tileDto.SpriteColor.B, tileDto.SpriteColor.A),
                                Rect = new Rect(tileDto.SpriteRect.X, tileDto.SpriteRect.Y, tileDto.SpriteRect.Width, tileDto.SpriteRect.Height),
                                LayerDepth = tileDto.LayerDepth,
                            }
                        };
                        tile.FrameCount = tileDto.FrameCount;
                    }

                    layer.Tiles[entry.X, entry.Y] = tile;
                }

                map.AddLayer(layerDto.Name, layer);
            }

            return map;
        }

        public override void Save(IContentModel descriptor, IDataManagerArguments arguments)
        {
            var mapDesc = (IMapModel<ILayerModel<ITileModel<SpriteInfo>>>)descriptor;
            string filePath = this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.MAP_FILE_EXT;

            var layerDtos = new List<LayerDto>();
            foreach (var layer in mapDesc.Layers)
            {
                var tileEntries = new List<TileEntryDto>();
                for (int x = 0; x < layer.Tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < layer.Tiles.GetLength(1); y++)
                    {
                        var tile = layer.Tiles[x, y];
                        if (tile == null)
                            continue;

                        string attrData = tile.Attribute != null
                            ? Convert.ToBase64String(tile.Attribute.Serialize())
                            : null;

                        TileDto tileDto;
                        if (tile.Sprite != null)
                        {
                            var r = tile.Sprite.Transform.Rect;
                            var c = tile.Sprite.Transform.Color;
                            tileDto = new TileDto(
                                attrData,
                                tile.Animated,
                                tile.LightSource,
                                tile.Sprite.TextureName,
                                new RectDto(r.X, r.Y, r.Width, r.Height),
                                new ColorDto(c.R, c.G, c.B, c.A),
                                tile.Sprite.Transform.LayerDepth,
                                tile.FrameCount
                            );
                        }
                        else
                        {
                            tileDto = new TileDto(attrData, false, false, null, null, null, 0f, 0);
                        }

                        tileEntries.Add(new TileEntryDto(x, y, tileDto));
                    }
                }

                layerDtos.Add(new LayerDto(layer.Name, layer.LayerIndex, tileEntries));
            }

            var mapDto = new MapDto(
                new List<string>(mapDesc.TilesetPaths),
                mapDesc.Name,
                new VectorDto(mapDesc.Dimensions.X, mapDesc.Dimensions.Y),
                mapDesc.Dark,
                layerDtos
            );

            File.WriteAllText(filePath, JsonSerializer.Serialize(mapDto, JsonOptions));
        }

        public override bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.MAP_FILE_EXT);
        }
    }
}
