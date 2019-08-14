using Lunar.Core.Utilities;
using Microsoft.Xna.Framework.Content;

namespace Lunar.Graphics
{
    public class ContentManagerService : IService
    {
        public ContentManager ContentManager { get; }

        public ContentManagerService(ContentManager contentManager)
        {
            this.ContentManager = contentManager;
        }

        public void Initalize()
        {
        }
    }
}
