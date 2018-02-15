using Microsoft.Xna.Framework.Content;
using Lunar.Core.Utilities;

namespace Lunar.Client.Utilities.Services
{
    class ContentManagerService : IService
    {
        private ContentManager _contemtManager;

        public ContentManager ContentManager => _contemtManager;

        public ContentManagerService(ContentManager contemtManager)
        {
            _contemtManager = contemtManager;
        }

        public void Initalize()
        {
            throw new System.NotImplementedException();
        }
    }
}
