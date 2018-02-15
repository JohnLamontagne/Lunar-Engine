using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Lunar.Editor.Controls
{
    class SpriteFileNameEditor : FileNameEditor
    {
        protected override void InitializeDialog(OpenFileDialog openFileDialog)
        {
            base.InitializeDialog(openFileDialog);
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "PNG|*.png";
        }
    }
}
