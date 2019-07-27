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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Structure;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class MapFSDataManager : FSDataManager<MapDescriptor>
    {
        public MapFSDataManager()
        {
            
        }

        public override MapDescriptor Load(IDataManagerArguments arguments)
        {
            MapDescriptor map = null;

            var mapArguments = (arguments as MapFSDataManagerArguments);

            using (var fileStream = new FileStream(this.RootPath + mapArguments.Name + EngineConstants.MAP_FILE_EXT, FileMode.Open))
            {
                using (var bR = new BinaryReader(fileStream))
                {
                    // Load the tileset information
                    int tilesetCount = bR.ReadInt32();
                    List<string> tilesetPaths = new List<string>();
                    for (int i = 0; i < tilesetCount; i++)
                    {
                        // We can throw this information away as it is used only in the editor suite.
                        string tilesetPath = bR.ReadString();
                        tilesetPaths.Add(tilesetPath);
                    }

                    string name = bR.ReadString();
                    var dimensions = new Vector(bR.ReadInt32(), bR.ReadInt32());

                    map = new MapDescriptor(dimensions, name)
                    {
                        Dark = bR.ReadBoolean()
                    };
                    map.TilesetPaths.AddRange(tilesetPaths);


                    map.Bounds = new Rect(0, 0, (int)map.Dimensions.X, (int)map.Dimensions.Y);

                    int layerCount = bR.ReadInt32();
                    for (int i = 0; i < layerCount; i++)
                    {
                        string layerName = bR.ReadString();
                        int lIndex = bR.ReadInt32();

                        var layer = new LayerDescriptor(map.Dimensions, layerName, lIndex);

                        for (int x = 0; x < layer.Tiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < layer.Tiles.GetLength(1); y++)
                            {
                                if (bR.ReadBoolean())
                                {

                                    layer.Tiles[x, y] = new TileDescriptor(new Vector(x * EngineConstants.TILE_SIZE,
                                        y * EngineConstants.TILE_SIZE))
                                    {
                                        Attribute = (TileAttributes)bR.ReadByte()
                                    };

                                    int attributeDataLength = bR.ReadInt32();
                                    byte[] attributeData = bR.ReadBytes(attributeDataLength);
                                    layer.Tiles[x, y].AttributeData = AttributeData.Deserialize(attributeData);

                                    if (bR.ReadBoolean())
                                    {
                                        layer.Tiles[x, y].Animated = bR.ReadBoolean();
                                        layer.Tiles[x, y].LightSource = bR.ReadBoolean();

                                        string spriteName = bR.ReadString();
                                        float zIndex = bR.ReadSingle(); // We can throw this away

                                        layer.Tiles[x, y].SpriteInfo = new SpriteInfo(spriteName)
                                        {
                                            Transform =
                                            {
                                                Position = new Vector(x * EngineConstants.TILE_SIZE, y * EngineConstants.TILE_SIZE),
                                                Color = new Color(bR.ReadByte(), bR.ReadByte(), bR.ReadByte(), bR.ReadByte()),
                                                Rect = new Rect(bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32()),
                                                LayerDepth = zIndex
                                            }
                                        };

                                        layer.Tiles[x, y].FrameCount = bR.ReadInt32();
                                    }
                                }
                            }
                        }

                        //int mapObjectCount = bR.ReadInt32();
                        //for (int mI = 0; mI < mapObjectCount; mI++)
                        //{
                        //    var mapObject = new MapObjectDescriptor()
                        //    {
                        //        Position = new Vector(bR.ReadSingle(), bR.ReadSingle())
                        //    };

                        //    if (bR.ReadBoolean())
                        //    {
                        //        string texturePath = bR.ReadString();
                        //        mapObject.Sprite = new SpriteInfo(texturePath)
                        //        {
                        //            Transform =
                        //            {
                        //                Rect = new Rect(bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32())
                        //            }
                        //        };
                        //    }

                        //    mapObject.Animated = bR.ReadBoolean();

                        //    mapObject.FrameTime = bR.ReadInt32();

                        //    string scriptPath = bR.ReadString();

                        //    var lightSource = bR.ReadBoolean();
                        //    var lightRadius = bR.ReadSingle();
                        //    var lightColor = new Color(bR.ReadByte(), bR.ReadByte(), bR.ReadByte(), bR.ReadByte());

                        //    if (lightSource)
                        //    {
                        //        mapObject.LightInformation = new LightInformation()
                        //        {
                        //            Radius = lightRadius,
                        //            Color = lightColor
                        //        };
                        //    }

                        //}

                        map.Layers.Add(layerName, layer);
                    }
                }
            }

            return map;
        }

        public override void Save(IContentDescriptor descriptor, IDataManagerArguments arguments)
        {
            MapDescriptor mapDesc = (MapDescriptor)descriptor;

            string filePath = this.RootPath + (arguments as MapFSDataManagerArguments).Name + EngineConstants.MAP_FILE_EXT;

            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var bW = new BinaryWriter(fileStream))
                {
                    bW.Write(mapDesc.TilesetPaths.Count);

                    foreach (var tilesetPath in mapDesc.TilesetPaths)
                    {
                        bW.Write(tilesetPath);
                    }

                    bW.Write(mapDesc.Name);

                    bW.Write((int)mapDesc.Dimensions.X);
                    bW.Write((int)mapDesc.Dimensions.Y);

                    bW.Write(mapDesc.Dark);

                    bW.Write(mapDesc.Layers.Count);

                    foreach (var layer in mapDesc.Layers.Values)
                    {
                        bW.Write(layer.Name);
                        bW.Write(layer.LayerIndex);

                        for (int x = 0; x < layer.Tiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < layer.Tiles.GetLength(1); y++)
                            {
                                if (layer.Tiles[x, y] != null)
                                {
                                    bW.Write(true);

                                    bW.Write((byte)layer.Tiles[x, y].Attribute);

                                    var attributeData = layer.Tiles[x, y].AttributeData.Serialize();

                                    bW.Write(attributeData.Length);
                                    bW.Write(attributeData);

                                    if (layer.Tiles[x, y].SpriteInfo != null)
                                    {
                                        bW.Write(true);

                                        bW.Write(layer.Tiles[x, y].Animated);
                                        bW.Write(layer.Tiles[x, y].LightSource);

                                        bW.Write(layer.Tiles[x, y].SpriteInfo.TextureName);

                                        bW.Write(layer.Tiles[x, y].SpriteInfo.Transform.LayerDepth);


                                        bW.Write(layer.Tiles[x, y].SpriteInfo.Transform.Color.R);
                                        bW.Write(layer.Tiles[x, y].SpriteInfo.Transform.Color.G);
                                        bW.Write(layer.Tiles[x, y].SpriteInfo.Transform.Color.B);
                                        bW.Write(layer.Tiles[x, y].SpriteInfo.Transform.Color.A);

                                        bW.Write(layer.Tiles[x, y].SpriteInfo.Transform.Rect.Left);
                                        bW.Write(layer.Tiles[x, y].SpriteInfo.Transform.Rect.Top);
                                        bW.Write(layer.Tiles[x, y].SpriteInfo.Transform.Rect.Width);
                                        bW.Write(layer.Tiles[x, y].SpriteInfo.Transform.Rect.Height);

                                        bW.Write(layer.Tiles[x, y].FrameCount);
                                    }
                                    else
                                    {
                                        bW.Write(false);
                                    }
                                }
                                else
                                {
                                    bW.Write(false);
                                }
                            }
                        }

                        
                    }

                    
                }
            }
        }

        public override bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(this.RootPath + (arguments as MapFSDataManagerArguments).Name + EngineConstants.MAP_FILE_EXT);
        }
    }
}
