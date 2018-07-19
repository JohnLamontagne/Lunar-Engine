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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DarkUI.Docking;
using Lunar.Editor.World;

namespace Lunar.Editor.Controls
{
    public partial class DockLayers : DarkToolWindow
    {
        private int _indexBefore;
        private bool _dragDrop;
        private Point _mouseLocation;

        private Map _map;

        public string SelectedLayer
        {
            get
            {
                if (this.lstLayers.SelectedItem != null)
                    return lstLayers.SelectedItem.ToString();
                else
                    return "null";
            }
        }

        public string[] Layers => this.lstLayers.Items.Cast<string>().Select(l => l.ToString()).ToArray();
                                  

        public DockLayers()
        {
            InitializeComponent();

            this.lstLayers.ItemCheck += (sender, args) =>
            {
                if (_map != null && _map.Layers.ContainsKey(this.lstLayers.Items[args.Index].ToString()))
                    _map.Layers[this.lstLayers.Items[args.Index].ToString()].Visible = args.NewValue == CheckState.Checked;
            };
        }

        public void SetMapSubject(Map map)
        {
            if (_map == map)
                return;

            _map = map;

            this.lstLayers.Items.Clear();

            foreach (var layer in _map.Layers.Values)
            {
                this.lstLayers.Items.Add(layer.Name, true);
            }

            this.lstLayers.SelectedItem = this.lstLayers.Items[0];
            this.lstLayers.SetItemChecked(0, true);
        }

        public void AddLayer(string layerName)
        {
            if (_map.Layers.ContainsKey(layerName))
            {
                return;
            }

            this.lstLayers.Items.Add(layerName);

            this.lstLayers.SelectedItem = this.lstLayers.Items[this.lstLayers.Items.Count - 1];
            this.lstLayers.SetItemChecked(this.lstLayers.Items.Count - 1, true);

            _map.Layers.Add(layerName, new Layer(_map.Dimensions, layerName, _map.Layers.Count + 1));
        }

        private void buttonAddLayer_Click(object sender, EventArgs e)
        {
            using (var dialog = new CreateLayerDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.AddLayer(dialog.LayerName);
                }
            }
        }

        private void lstLayers_MouseDown(object sender, MouseEventArgs e)
        {
            int itemIndex = this.lstLayers.IndexFromPoint(e.X, e.Y);
            if (itemIndex >= 0 & e.Button == MouseButtons.Left)
            {
                _dragDrop = true;
                _mouseLocation = e.Location;
            }
        }

        private void lstLayers_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void lstLayers_DragDrop(object sender, DragEventArgs e)
        {
            Point point = this.lstLayers.PointToClient(new Point(e.X, e.Y));
            int index = this.lstLayers.IndexFromPoint(point);
            if (index < 0) index = this.lstLayers.Items.Count - 1;
            if (index == _indexBefore)
            {
                this.lstLayers.SetItemChecked(index, !this.lstLayers.GetItemChecked(index));
                return;
            }
            object data = e.Data.GetData(typeof(string));
            bool checkState = this.lstLayers.GetItemChecked(_indexBefore);
            this.lstLayers.Items.Remove(data);
            this.lstLayers.Items.Insert(index, data);
            this.lstLayers.SetItemChecked(index, checkState);


            int lIndex = 0;
            foreach (var layerName in this.Layers)
            {
                _map.Layers[layerName].LayerIndex = lIndex++;
            }
        }

        private void lstLayers_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragDrop || this.lstLayers.SelectedItem == null || e.Location == _mouseLocation)
                return;

            _indexBefore = this.lstLayers.SelectedIndex;
            _dragDrop = false;
            this.lstLayers.DoDragDrop(this.lstLayers.SelectedItem, DragDropEffects.Move);
        }
    }
}
