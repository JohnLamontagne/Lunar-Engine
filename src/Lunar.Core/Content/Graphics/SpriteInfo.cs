namespace Lunar.Core.Content.Graphics
{
    public class SpriteInfo
    {
        public Transform Transform { get; set; }

        public string TextureName { get; set; }

        public SpriteInfo(string textureName)
        {
            this.TextureName = textureName;
            this.Transform = new Transform();
        }
    }
}
