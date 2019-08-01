using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Client.GUI.Widgets
{
    public interface ILexicalWidget : IWidget
    {
        SpriteFont Font { get; set; }
    }
}