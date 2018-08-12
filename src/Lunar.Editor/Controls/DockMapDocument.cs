/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Forms;
using Lunar.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Core.Utilities.Logic;
using Lunar.Editor.Utilities;
using Lunar.Editor.World;
using Color = Microsoft.Xna.Framework.Color;
using Keys = System.Windows.Forms.Keys;
using Lunar.Core.World;
using Lunar.Core.World.Structure;
using Lunar.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Lunar.Editor.Controls
{
    public partial class DockMapDocument : SavableDocument
    {
        private FileInfo _file;
        private string _regularDockText;
        private string _unsavedDockText;
        private bool _unsaved;
        private Map _map;
        private Project _project;
        private Camera _camera;
        private Vector2 _mapMousePos;
        private TextureLoader _mapTextureLoader;

        private DockTilesetTools _dockTilesetTools;
        private DockLayers _dockLayers;
        private DockMapObjectProperties _dockMapObject;
        private DockMapAttributes _dockMapAttributes;


        /// <summary>
        /// Used when selecting tiles for attribute data.
        /// </summary>
        private WarpAttributeDialog _tileAttributeDialog;

        private PlacementMode _prevPlacementMode;


        private PlacementMode _placementMode;
        private bool _mapDragging;

        private Texture2D _pointTexture;
        private Vector2 _selectPosition;
        private Rectangle _selectRectangle;

        private MapObject _selectedMapObject;
        private Vector2 _selectedMapObjectOffset;

        private Dictionary<Vector3, Sprite> _tileAttributeSprites;

        public Map Map => _map;

        public DockMapDocument()
        {
            InitializeComponent();
        }

        public DockMapDocument(string text, Image icon, FileInfo file, Project project, 
            DockTilesetTools dockTilesetTools, DockLayers dockLayers, DockMapObjectProperties dockMapObject, DockMapAttributes dockMapAttributes)
            : this()
        {
            _regularDockText = text;
            _unsavedDockText = text + "*";

            DockText = text;
            Icon = icon;

            _file = file;
            _project = project;
            _dockLayers = dockLayers;
            _dockTilesetTools = dockTilesetTools;
            _dockMapObject = dockMapObject;
            _dockMapAttributes = dockMapAttributes;
            _dockMapAttributes.SetProject(project);

            _dockTilesetTools.Tileset_Loaded += _tilesetTools_Tileset_Loaded;
            _dockTilesetTools.Tileset_Unloaded += _tilesetTools_Tileset_Unloaded;

            _dockMapAttributes.SelectingTile += DockMapAttributesOnSelectingTile;

            this.mapToolStrip.Items[1].Image = Icons.BrushSelected;
            this.mapView.Cursor = new Cursor(Icons.Brush.GetHicon());
            _placementMode = PlacementMode.Paint;

            this.mapView.Resize += MapView_Resize;

            _tileAttributeSprites = new Dictionary<Vector3, Sprite>();
        }

        private void DockMapAttributesOnSelectingTile(object sender, EventArgs eventArgs)
        {
            _tileAttributeDialog = (WarpAttributeDialog) sender;
            _prevPlacementMode = _placementMode;
            _placementMode = PlacementMode.Picking_Tile;


            if (_tileAttributeDialog.MapSubject == _map)
            {
                _tileAttributeDialog.Submitted += _tileAttributeDialog_Submitted;
            }
            else
            {
                _tileAttributeDialog.Submitted+= TileAttributeDialogOnSubmitted_Inactive;
            }
        }

        private void TileAttributeDialogOnSubmitted_Inactive(object sender, EventArgs eventArgs)
        {
            _placementMode = _prevPlacementMode;
        }

        private void _tileAttributeDialog_Submitted(object sender, EventArgs e)
        {
            this.Show();

            _placementMode = _prevPlacementMode;
            _tileAttributeDialog.Submitted -= _tileAttributeDialog_Submitted;
        }

        private void MapView_Resize(object sender, EventArgs e)
        {
            if (_camera != null)
                _camera.Bounds = new Rectangle(0, 0, this.mapView.Width, this.mapView.Height);
        }

        public override void Close()
        {
            if (_unsaved)
            {
                var result = DarkMessageBox.ShowWarning(@"You will lose any unsaved changes. Continue?", @"Close document", DarkDialogButton.YesNo);
                if (result == DialogResult.No)
                    return;
            }

            base.Close();
        }

        private void _tilesetTools_Tileset_Unloaded(object sender, DockTilesetTools.TilesetLoadedEventArgs e)
        {
            string tilesetName = Path.GetFileName(e.TilesetPath);

            if (_dockTilesetTools.Map == _map)
            {
                foreach (var layer in _map.Layers.Values)
                {
                    for (int x = 0; x < _map.Dimensions.X; x++)
                    {
                        for (int y = 0; y < _map.Dimensions.Y; y++)
                        {
                            if (layer.GetTile(x, y) != null && layer.GetTile(x, y).Sprite.Texture == _map.Tilesets[tilesetName])
                            {
                                layer.SetTile(x, y, new Tile());
                            }
                        }
                    }
                }
            }

            _map.Tilesets.Remove(tilesetName);
            this.MarkUnsaved();
        }


        private void _tilesetTools_Tileset_Loaded(object sender, DockTilesetTools.TilesetLoadedEventArgs e)
        {
            if (_dockTilesetTools.Map == _map)
            {
                var mapTilesetTexture = _mapTextureLoader.LoadFromFile(e.TilesetPath);
                mapTilesetTexture.Tag = HelperFunctions.MakeRelative(e.TilesetPath, _project.ClientRootDirectory.FullName + "/");

                _map.Tilesets.Add(Path.GetFileName(e.TilesetPath), mapTilesetTexture);

                this.MarkUnsaved();
            }
        }

        private void LoadMapDocument()
        {
            _map = _project.LoadMap(_file.FullName, _mapTextureLoader);
            _map.Map_Resized += _map_Map_Resized;

            this.scrollX.Maximum = Math.Max(((int)_map.Dimensions.X * Constants.TILE_SIZE) - this.mapView.Width, 0);
            this.scrollY.Maximum = Math.Max(((int)_map.Dimensions.Y * Constants.TILE_SIZE) - this.mapView.Height, 0);

            this.mapView.OnDraw = OnMapDraw;
            this.mapView.OnUpdate = OnMapUpdate;

            _dockLayers.SetMapSubject(_map);
            

            foreach (var layer in _map.Layers.Values.OrderBy(l => l.ZIndex))
            {
                _dockLayers.AddLayer(layer.Name);
            }

            MemoryStream memStream = new MemoryStream();
            Icons.NullObject.Save(memStream, ImageFormat.Png);

            var texture = _mapTextureLoader.LoadFromFileStream(memStream);

            // Grab all the needed attribute information for the map display
            foreach (var layer in _map.Layers.Values)
            {
                for (int x = 0; x < _map.Dimensions.X; x++)
                {
                    for (int y = 0; y < _map.Dimensions.Y; y++)
                    {
                        if (layer.GetTile(x, y) != null && layer.GetTile(x, y).Attribute != TileAttributes.None)
                        {
                            var attributeSprite = new Sprite(texture)
                            {
                                Position = new Vector2(x * Constants.TILE_SIZE, y * Constants.TILE_SIZE),
                                LayerDepth = layer.ZIndex + .02f // place it slightly above the layer's tile
                            };

                            switch (layer.GetTile(x, y).Attribute)
                            {
                                case TileAttributes.Blocked:
                                    attributeSprite.Color = new Color(Color.Red, 100);
                                    break;
                                case TileAttributes.PlayerSpawn:
                                    attributeSprite.Color = new Color(Color.Blue, 100);
                                    break;
                                case TileAttributes.Warp:
                                    attributeSprite.Color = new Color(Color.Purple, 100);
                                    break;
                                case TileAttributes.NPCSpawn:
                                    attributeSprite.Color = new Color(Color.DarkGreen, 100);
                                    break;
                            }

                            _tileAttributeSprites.Add(new Vector3(x, y, layer.ZIndex), attributeSprite);
                        }
                    }
                }
            }
        }

        private void _map_Map_Resized(object sender, EventArgs e)
        {
            this.scrollX.Maximum = Math.Max(((int)_map.Dimensions.X * Constants.TILE_SIZE) - this.mapView.Width, 0);
            this.scrollY.Maximum = Math.Max(((int)_map.Dimensions.Y * Constants.TILE_SIZE) - this.mapView.Height, 0);
        }

        private void OnMapUpdate(View view)
        {
            _map.Update(view.GameTime);
        }

        private void OnMapDraw(View view)
        {
            view.SpriteBatch.End();

            view.SpriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, _camera.GetTransformation());

            _map.Draw(view.SpriteBatch, _camera);

            // Draw a preview version of the selected tile(s
            if (_dockTilesetTools.SelectedTileset != null && _map.Tilesets.ContainsKey(_dockTilesetTools.SelectedTileset.ToString()) && (_placementMode == PlacementMode.Paint || _placementMode == PlacementMode.Fill))
            {
                var currentTileset = _map.Tilesets[_dockTilesetTools.SelectedTileset.ToString()];

                Vector2 placeTilePos = new Vector2(((int)(_mapMousePos.X + _camera.Position.X) / Constants.TILE_SIZE) * Constants.TILE_SIZE,
                    ((int)(_mapMousePos.Y + _camera.Position.Y) / Constants.TILE_SIZE) * Constants.TILE_SIZE);

                if (_map.Layers.ContainsKey(_dockLayers.SelectedLayer))
                    view.SpriteBatch.Draw(currentTileset, placeTilePos, _dockTilesetTools.SelectRectangle, 
                        new Color(Color.White, 150), 0f, Vector2.Zero, 1f, SpriteEffects.None, _map.Layers[_dockLayers.SelectedLayer].ZIndex);
            }
            else if (_placementMode == PlacementMode.Select || _placementMode == PlacementMode.MapObject_Select)
            {
                this.DrawRectangle(view.SpriteBatch, _selectRectangle, Color.Red, 3);
            }

            if (_selectedMapObject != null)
                _selectRectangle = new Rectangle((int)_selectedMapObject.Position.X - 1, (int)_selectedMapObject.Position.Y - 1,
                    _selectedMapObject.Sprite.SourceRectangle.Width + 1, _selectedMapObject.Sprite.SourceRectangle.Height + 1);

            if (_placementMode == PlacementMode.Place_Attribute)
            {
                foreach (var attributeSprite in _tileAttributeSprites.Values)
                    view.SpriteBatch.Draw(attributeSprite);
            }
        }

        public override void Save()
        {
            _regularDockText = _map.Name + EngineConstants.MAP_FILE_EXT;

            this.DockText = _regularDockText;
            _unsaved = false;

            if (_map.Name + EngineConstants.MAP_FILE_EXT != _file.Name)
            {
                File.Move(_file.FullName, _file.DirectoryName + "/" + _map.Name + EngineConstants.MAP_FILE_EXT);

                _file = _project.ChangeItem(_file.FullName, _file.DirectoryName + "\\" + _map.Name + EngineConstants.MAP_FILE_EXT);
            }

            _map.Save(_file.FullName);
        }


        private void DockMapDocument_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control && e.KeyCode == Keys.S)
            {
                this.Save();
                e.SuppressKeyPress = true;
            } 
            else if (e.KeyCode == Keys.Delete)
            {
                if (_placementMode == PlacementMode.MapObject_Select && _selectedMapObject != null)
                {
                    _selectedMapObject.Layer.MapObjects.Remove(_selectedMapObject);
                }
            }
        }

        private void DockMapDocument_Load(object sender, System.EventArgs e)
        {
            _camera = new Camera(new Rectangle(0, 0, this.mapView.Width, this.mapView.Height));

            _mapTextureLoader = new TextureLoader(this.mapView.GraphicsDevice);
            
            this.LoadMapDocument();
        }

        private void mapView_MouseDown(object sender, MouseEventArgs e)
        {
            _mapDragging = true;

            if (_placementMode == PlacementMode.Paint)
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.PlaceTile((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE,
                        (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE);
                }
                else
                {
                    this.RemoveTile((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE,
                        (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE);
                }
            }
            else if (_placementMode == PlacementMode.Erase)
            {
                this.RemoveTile((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE, (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE);
            }
            else if (_placementMode == PlacementMode.Select)
            {
                _selectPosition = new Vector2((e.X / Constants.TILE_SIZE) * Constants.TILE_SIZE, (e.Y / Constants.TILE_SIZE) * Constants.TILE_SIZE);
                _selectPosition = new Vector2(_selectPosition.X + ((int)_camera.Position.X / Constants.TILE_SIZE) * Constants.TILE_SIZE, _selectPosition.Y + ((int)_camera.Position.Y / Constants.TILE_SIZE) * Constants.TILE_SIZE);
                _selectRectangle = new Rectangle((int)_selectPosition.X, (int)_selectPosition.Y, Constants.TILE_SIZE, Constants.TILE_SIZE);
            }
            else if (_placementMode == PlacementMode.MapObject_Select)
            {
                if (_map.Layers.ContainsKey(_dockLayers.SelectedLayer))
                {
                    _selectedMapObject = _map.Layers[_dockLayers.SelectedLayer].TryGetMapObject(new Vector2(e.X, e.Y));

                    if (_selectedMapObject != null)
                    {
                        _dockMapObject.SetSubject(new MapObjectPropertiesHelper(_selectedMapObject, _mapTextureLoader, _project));
                        _selectedMapObjectOffset = new Vector2(e.X - _selectedMapObject.Position.X, e.Y - _selectedMapObject.Position.Y);
                    }
                }
               
            }
        }
        
        private void mapView_MouseMove(object sender, MouseEventArgs e)
        {
            _mapMousePos = new Vector2(e.Location.X , e.Location.Y);

            if (_mapDragging)
            {
                if (_placementMode == PlacementMode.Paint)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        this.PlaceTile((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE,
                            (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE);
                    }
                    else
                    {
                        this.RemoveTile((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE,
                            (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE);
                    }
                }
                else if (_placementMode == PlacementMode.Erase)
                {
                    this.RemoveTile((e.X + (int) _camera.Position.X) / Constants.TILE_SIZE,
                        (e.Y + (int) _camera.Position.Y) / Constants.TILE_SIZE);
                }
                else if (_placementMode == PlacementMode.Select)
                {
                    int left = (int) _selectPosition.X;
                    int top = (int) _selectPosition.Y;
                    int width = Math.Abs((int) ((e.X + _camera.Position.X) - _selectPosition.X));
                    int height = Math.Abs((int) ((e.Y + _camera.Position.Y) - _selectPosition.Y));

                    if ((e.X + _camera.Position.X) < _selectPosition.X)
                    {
                        left = (int) (e.X + _camera.Position.X);
                    }

                    if ((e.Y + _camera.Position.Y) < _selectPosition.Y)
                    {
                        top = (int) (e.Y + _camera.Position.Y);
                    }

                    _selectRectangle = new Rectangle(left, top, width, height);
                }
                else if (_placementMode == PlacementMode.Place_Attribute)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        this.PlaceTileAttribute((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE,
                            (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE, _dockMapAttributes.Attribute);
                    }
                    else
                    {
                        this.RemoveMapAttribute((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE,
                            (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE);
                    }
                }
                else if (_placementMode == PlacementMode.MapObject_Select)
                {
                    if (_selectedMapObject != null)
                    {
                        _selectedMapObject.Position = _mapMousePos - _selectedMapObjectOffset;
                    }
                }
            }
        }

        private void mapView_MouseClick(object sender, MouseEventArgs e)
        {
            if (_placementMode == PlacementMode.Paint)
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.PlaceTile((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE,
                        (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE);
                }
                else
                {
                    this.RemoveTile((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE,
                        (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE);
                }
            }
            else if (_placementMode == PlacementMode.Erase)
            {
                this.RemoveTile((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE, (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE);
            }
            else if (_placementMode == PlacementMode.Fill)
            {
                for (int x = 0; x < _map.Dimensions.X; x++)
                {
                    for (int y = 0; y < _map.Dimensions.Y; y++)
                    {
                        this.PlaceTile(x, y);
                    }
                }
            }
            else if (_placementMode == PlacementMode.MapObject)
            {
                this.PlaceMapObject((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE, (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE);
            } 
            else if (_placementMode == PlacementMode.Place_Attribute)
            {
                if (e.Button == MouseButtons.Left)
                {
                    this.PlaceTileAttribute((e.X + (int) _camera.Position.X) / Constants.TILE_SIZE,
                        (e.Y + (int) _camera.Position.Y) / Constants.TILE_SIZE, _dockMapAttributes.Attribute);
                }
                else
                {
                    this.RemoveMapAttribute((e.X + (int)_camera.Position.X) / Constants.TILE_SIZE,
                        (e.Y + (int)_camera.Position.Y) / Constants.TILE_SIZE);
                }
            }
            else if (_placementMode == PlacementMode.Picking_Tile)
            {
                _tileAttributeDialog.WarpX = (int)(e.X +  _camera.Position.X);
                _tileAttributeDialog.WarpY = (int) (e.Y + _camera.Position.Y);
                _tileAttributeDialog.WarpMapID = _map.Name;
                _tileAttributeDialog.WarpLayerName = _dockLayers.SelectedLayer;
                _tileAttributeDialog.Show();

                _placementMode = _prevPlacementMode;
            }
        }


        private void mapView_MouseUp(object sender, MouseEventArgs e)
        {
            if (_placementMode == PlacementMode.Select)
            {
                int mouseX = (int) (e.X + _camera.Position.X);
                int mouseY = (int)(e.Y + _camera.Position.Y);
       
                int left = (int)_selectPosition.X;
                int top = (int)_selectPosition.Y;


                int width = 0;
                if (mouseX < _selectPosition.X)
                {
                    left = mouseX;
                    width = (int)Math.Abs((((mouseX / Constants.TILE_SIZE)) * Constants.TILE_SIZE) -
                        _selectPosition.X);
                }
                else
                {
                    width = (int)Math.Abs((((mouseX / Constants.TILE_SIZE) + 1) * Constants.TILE_SIZE) -
                        _selectPosition.X);
                }
                width = width < Constants.TILE_SIZE ? Constants.TILE_SIZE : width;

                int height = 0;
                if (mouseY < _selectPosition.Y)
                {
                    top = mouseY;
                    height = (int)Math.Abs((((mouseY / Constants.TILE_SIZE)) * Constants.TILE_SIZE) - _selectPosition.Y);
                }
                else
                {
                    height = (int)Math.Abs((((mouseY / Constants.TILE_SIZE) + 1) * Constants.TILE_SIZE) - _selectPosition.Y);
                }
                height = height < Constants.TILE_SIZE ? Constants.TILE_SIZE : height;

                _selectRectangle = new Rectangle(left, top, width, height);
            }
                
            _mapDragging = false;
        }

        private void RemoveMapAttribute(int mapX, int mapY)
        {
            // Make sure the layer exists
            if (!_map.Layers.ContainsKey(_dockLayers.SelectedLayer))
                return;

            var layer = _map.Layers[_dockLayers.SelectedLayer];

            if (mapX >= 0 && mapY >= 0 && mapX < _map.Dimensions.X && mapY < _map.Dimensions.Y)
            {
                if (layer.GetTile(mapX, mapY) == null)
                    return;

                layer.GetTile(mapX, mapY).Attribute = TileAttributes.None;

                _tileAttributeSprites.Remove(new Vector3(mapX, mapY, layer.ZIndex));

                this.MarkUnsaved();
            }
        }

        private void PlaceTileAttribute(int mapX, int mapY, TileAttributes attribute)
        {
            // Make sure the layer exists
            if (!_map.Layers.ContainsKey(_dockLayers.SelectedLayer))
                return;

            var layer = _map.Layers[_dockLayers.SelectedLayer];

            if (mapX >= 0 && mapY >= 0 && mapX < _map.Dimensions.X && mapY < _map.Dimensions.Y)
            {
                if (layer.GetTile(mapX, mapY) == null)
                {
                    var tile = new Tile();
                    layer.SetTile(mapX, mapY, tile);
                }

                layer.GetTile(mapX, mapY).Attribute = attribute;
                layer.GetTile(mapX, mapY).AttributeData = _dockMapAttributes.AttributeData;

                MemoryStream memStream = new MemoryStream();
                Icons.NullObject.Save(memStream, ImageFormat.Png);

                var texture = _mapTextureLoader.LoadFromFileStream(memStream);

                var attributeSprite = new Sprite(texture)
                {
                    Position = new Vector2(mapX * Constants.TILE_SIZE, mapY * Constants.TILE_SIZE),
                    LayerDepth = layer.ZIndex + .02f // place it slightly above the layer's tile
                };
                
                switch (attribute)
                {
                    case TileAttributes.Blocked:
                        attributeSprite.Color = new Color(Color.Red, 100);
                        break;
                    case TileAttributes.PlayerSpawn:
                        attributeSprite.Color = new Color(Color.Blue, 100);
                        break;
                    case TileAttributes.Warp:
                        attributeSprite.Color = new Color(Color.Purple, 100);
                        break;
                    case TileAttributes.NPCSpawn:
                        attributeSprite.Color = new Color(Color.DarkGreen, 100);
                        break;
                    default:
                        attributeSprite.Color = Color.Transparent;
                        break;
                }

                var locationKey = new Vector3(mapX, mapY, layer.ZIndex);

                if (!_tileAttributeSprites.ContainsKey(locationKey))
                    _tileAttributeSprites.Add(locationKey, attributeSprite);
                else
                    _tileAttributeSprites[locationKey] = attributeSprite;

                this.MarkUnsaved();
            }
        }

        private void PlaceMapObject(int mapX, int mapY)
        {
            if (!_map.Layers.ContainsKey(_dockLayers.SelectedLayer))
                return;

            MemoryStream memStream = new MemoryStream();
            Icons.NullObject.Save(memStream, ImageFormat.Png);

            var texture = _mapTextureLoader.LoadFromFileStream(memStream);
                
            var mapObject = new MapObject(new Vector2(mapX * Constants.TILE_SIZE, mapY * Constants.TILE_SIZE), _map.Layers[_dockLayers.SelectedLayer])
            {
                Sprite = new Sprite(texture)
                {
                    LayerDepth = _map.Layers[_dockLayers.SelectedLayer].ZIndex + .01f, // Display the map objects slightly above the layer
                }
            };

            mapObject.Sprite.Texture.Tag = "null";

            _map.Layers[_dockLayers.SelectedLayer].MapObjects.Add(mapObject);

            _dockMapObject.SetSubject(new MapObjectPropertiesHelper(mapObject, _mapTextureLoader, _project));

            this.MarkUnsaved();
        }

        private void RemoveTile(int mapX, int mapY)
        {
            if (!_map.Layers.ContainsKey(_dockLayers.SelectedLayer))
                return;

            if (mapX >= 0 && mapY >= 0 && mapX < _map.Dimensions.X && mapY < _map.Dimensions.Y)
            {
                _map.Layers[_dockLayers.SelectedLayer].SetTile(mapX, mapY, new Tile());

                this.MarkUnsaved();
            }
        }

        private void PlaceTile(int mapX, int mapY)
        {
            int tilesetIndex = _dockTilesetTools.SelectedTilesetIndex;

            if (tilesetIndex < 0 || tilesetIndex >= _dockTilesetTools.Tilesets.Count)
                return;

            var tilesetTexture2D = _map.Tilesets[_dockTilesetTools.SelectedTileset.ToString()];

            int placeTilesWidth = (int)Math.Round(_dockTilesetTools.SelectRectangle.Width / (float)Constants.TILE_SIZE);
            int placeTilesHeight = (int)Math.Round(_dockTilesetTools.SelectRectangle.Height / (float)Constants.TILE_SIZE);

            // Make sure the layer exists
            if (!_map.Layers.ContainsKey(_dockLayers.SelectedLayer))
                return;

            var layer = _map.Layers[_dockLayers.SelectedLayer];

            int tSetX = _dockTilesetTools.SelectRectangle.X;
            int tSetY = _dockTilesetTools.SelectRectangle.Y;
            
            for (int x = mapX; x < mapX + placeTilesWidth; x++)
            {
                for (int y = mapY; y < mapY + placeTilesHeight; y++)
                {
                    if (x >= 0 && y >= 0 && x < _map.Dimensions.X && y < _map.Dimensions.Y)
                    {
                        // Create a new tile at the specified x & y
                        Tile tile = new Tile(tilesetTexture2D, new Rectangle(tSetX, tSetY, Constants.TILE_SIZE, Constants.TILE_SIZE),
                            new Vector2(x * Constants.TILE_SIZE, y * Constants.TILE_SIZE))
                        {
                            ZIndex = layer.ZIndex
                        };

                        layer.SetTile(x, y, tile);
                        
                        // Increment the tileset y value so that we're pulling the correct tile when placing
                        tSetY += Constants.TILE_SIZE;
                    }
                }

                // Increment the tileset x value so that we're pulling the correct tile when placing
                if (tSetX < (_dockTilesetTools.SelectRectangle.X + _dockTilesetTools.SelectRectangle.Width) - Constants.TILE_SIZE)
                    tSetX += Constants.TILE_SIZE;

                // Reset the tileset y value so we begin picking from the correct place in the tileset next y loop
                tSetY = _dockTilesetTools.SelectRectangle.Y;
            }

            this.MarkUnsaved();
        }

        private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int lineWidth)
        {
            if (_pointTexture == null)
            {
                _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData<Color>(new Color[] { Color.White });
            }

            spriteBatch.Draw(_pointTexture, null, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), null, null, 0f, null, color, SpriteEffects.None, 1f);
            spriteBatch.Draw(_pointTexture, null, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), null, null, 0f, null, color, SpriteEffects.None, 1f);
            spriteBatch.Draw(_pointTexture, null, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), null, null, 0f, null, color, SpriteEffects.None, 1f);
            spriteBatch.Draw(_pointTexture, null, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), null, null, 0f, null, color, SpriteEffects.None, 1f);
        }



        private void fillBucketButton_Click(object sender, EventArgs e)
        {
            this.mapToolStrip.Items[0].Image = Icons.BucketSelected;
            this.mapToolStrip.Items[1].Image = Icons.Brush;
            this.mapToolStrip.Items[2].Image = Icons.Stamp;
            this.mapToolStrip.Items[3].Image = Icons.Eraser;
            this.mapToolStrip.Items[4].Image = Icons.Select;
            this.mapView.Cursor = new Cursor(Icons.Bucket.GetHicon());
            _placementMode = PlacementMode.Fill;
        }

        private void brushButton_Click(object sender, EventArgs e)
        {
            this.mapToolStrip.Items[0].Image = Icons.Bucket;
            this.mapToolStrip.Items[1].Image = Icons.BrushSelected;
            this.mapToolStrip.Items[2].Image = Icons.Stamp;
            this.mapToolStrip.Items[3].Image = Icons.Eraser;
            this.mapToolStrip.Items[4].Image = Icons.Select;
            this.mapView.Cursor = new Cursor(Icons.Brush.GetHicon());
            _placementMode = PlacementMode.Paint;
        }

        private void eraserButton_Click(object sender, EventArgs e)
        {
            this.mapToolStrip.Items[0].Image = Icons.Bucket;
            this.mapToolStrip.Items[1].Image = Icons.Brush;
            this.mapToolStrip.Items[2].Image = Icons.Stamp;
            this.mapToolStrip.Items[3].Image = Icons.EraserSelected;
            this.mapToolStrip.Items[4].Image = Icons.Select;
            this.mapView.Cursor = new Cursor(Icons.Eraser.GetHicon());
            _placementMode = PlacementMode.Erase;
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            _selectRectangle = Rectangle.Empty;

            this.mapToolStrip.Items[0].Image = Icons.Bucket;
            this.mapToolStrip.Items[1].Image = Icons.Brush;
            this.mapToolStrip.Items[2].Image = Icons.Stamp;
            this.mapToolStrip.Items[3].Image = Icons.Eraser;
            this.mapToolStrip.Items[4].Image = Icons.SelectSelected;
            this.mapView.Cursor = new Cursor(Icons.Select.GetHicon());
            _placementMode = PlacementMode.Select;
        }

        private void buttonMapObject_Click(object sender, EventArgs e)
        {
            this.mapToolStrip.Items[0].Image = Icons.Bucket;
            this.mapToolStrip.Items[1].Image = Icons.Brush;
            this.mapToolStrip.Items[2].Image = Icons.Stamp;
            this.mapToolStrip.Items[3].Image = Icons.Eraser;
            this.mapToolStrip.Items[4].Image = Icons.Select;
            this.mapView.Cursor = new Cursor(Icons.document_16xLG.GetHicon());
            _placementMode = PlacementMode.MapObject;
        }

        private void toolSelectObjectButton_Click(object sender, EventArgs e)
        {
            _selectRectangle = Rectangle.Empty;

            this.mapToolStrip.Items[0].Image = Icons.Bucket;
            this.mapToolStrip.Items[1].Image = Icons.Brush;
            this.mapToolStrip.Items[2].Image = Icons.Stamp;
            this.mapToolStrip.Items[3].Image = Icons.Eraser;
            this.mapToolStrip.Items[4].Image = Icons.Select;
            this.mapView.Cursor = new Cursor(Icons.cursor.GetHicon());
            _placementMode = PlacementMode.MapObject_Select;
        }

        private void buttonAttribute_Click(object sender, EventArgs e)
        {
            this.mapToolStrip.Items[0].Image = Icons.Bucket;
            this.mapToolStrip.Items[1].Image = Icons.Brush;
            this.mapToolStrip.Items[2].Image = Icons.StampSelected;
            this.mapToolStrip.Items[3].Image = Icons.Eraser;
            this.mapToolStrip.Items[4].Image = Icons.Select;
            this.mapView.Cursor = new Cursor(Icons.Stamp.GetHicon());
            _placementMode = PlacementMode.Place_Attribute;
        }

        private void scrollX_ValueChanged(object sender, ScrollValueEventArgs e)
        {
            _camera.Position = new Vector2(e.Value, _camera.Position.Y);
        }

        private void scrollY_ValueChanged(object sender, ScrollValueEventArgs e)
        {
            _camera.Position = new Vector2(_camera.Position.X, e.Value);
        }

        private void mapView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && _placementMode == PlacementMode.Select)
            {
                int startX = (int) _selectPosition.X / Constants.TILE_SIZE;
                int startY = (int) _selectPosition.Y / Constants.TILE_SIZE;
                int width = _selectRectangle.Width / Constants.TILE_SIZE;
                int height = _selectRectangle.Height / Constants.TILE_SIZE;

                for (int x = startX; x < startX + width; x++)
                {
                    for (int y = startY; y < startY + height; y++)
                    {
                        this.RemoveTile(x, y);
                    }
                }
            }
            else if ((Control.ModifierKeys & Keys.Control) == Keys.Control && e.KeyCode == Keys.A && _placementMode == PlacementMode.Select)
            {
                _selectPosition = Vector2.Zero;
                _selectRectangle = new Rectangle(0, 0, (int)_map.Dimensions.X * Constants.TILE_SIZE, (int)_map.Dimensions.Y * Constants.TILE_SIZE);
            }

            this.OnKeyDown(e);
        }

        private void mapToolStrip_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        private void scrollY_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        private void scrollX_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        private void MarkUnsaved()
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;
        }

        private enum PlacementMode
        {
            Paint,
            Fill,
            Select,
            MapObject,
            MapObject_Select,
            Place_Attribute,
            Picking_Tile,
            Erase
        }
    }
}
