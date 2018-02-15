using System;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Core.Utilities;

namespace Lunar.Client.Utilities.Services
{
    public class GraphicsDeviceService : IService
    {
        private GraphicsDevice _graphicsDevice;

        public GraphicsDevice GraphicsDevice => _graphicsDevice;

        public GraphicsDeviceService(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void Initalize()
        {
            throw new NotImplementedException();
        }
    }
}
