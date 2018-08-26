using System.IO;
using Lunar.Core;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Core.World.Structure;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class MapFSDataManager : IDataManager<MapDescriptor>
    {
        public MapFSDataManager()
        {
            
        }

        public MapDescriptor Load(IDataManagerArguments arguments)
        {
            MapDescriptor map = null;

            string path = Core.EngineConstants.FILEPATH_MAPS + (arguments as MapDataLoaderArguments)?.Name + EngineConstants.MAP_FILE_EXT;

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                using (var bR = new BinaryReader(fileStream))
                {
                    // Load the tileset information
                    int tilesetCount = bR.ReadInt32();
                    for (int i = 0; i < tilesetCount; i++)
                    {
                        // We can throw this information away as it is used only in the editor suite.
                        string tilesetPath = bR.ReadString();
                    }

                    string name = bR.ReadString();
                    var dimensions = new Vector(bR.ReadInt32(), bR.ReadInt32());

                    map = new MapDescriptor(dimensions, name)
                    {
                        Dark = bR.ReadBoolean()
                    };


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

                                    layer.Tiles[x, y] = new TileDescriptor(new Vector(x * EngineConstants.TILE_WIDTH,
                                        y * EngineConstants.TILE_HEIGHT))
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
                                                Position = new Vector(x * EngineConstants.TILE_WIDTH, y * EngineConstants.TILE_HEIGHT),
                                                Color = new Color(bR.ReadByte(), bR.ReadByte(), bR.ReadByte(), bR.ReadByte()),
                                                Rect = new Rect(bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32())
                                            }
                                        };

                                        layer.Tiles[x, y].FrameCount = bR.ReadInt32();
                                    }
                                }
                            }
                        }

                        int mapObjectCount = bR.ReadInt32();
                        for (int mI = 0; mI < mapObjectCount; mI++)
                        {
                            var mapObject = new MapObjectDescriptor()
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

                            string scriptPath = bR.ReadString();

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

                        }

                        map.Layers.Add(layerName, layer);
                    }
                }
            }

            return map;
        }

        public void Save(IDataDescriptor descriptor)
        {
            throw new System.NotImplementedException();
        }

        public bool Exists(IDataManagerArguments arguments)
        {
            throw new System.NotImplementedException();
        }
    }
}
