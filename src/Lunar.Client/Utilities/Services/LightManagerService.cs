using System;
using Penumbra;
using Lunar.Core.Utilities;

namespace Lunar.Client.Utilities.Services
{
    public class LightManagerService : IService
    {
        private PenumbraComponent _component;

        public PenumbraComponent Component => _component;

        public LightManagerService(PenumbraComponent component)
        {
            _component = component;
        }

        public void Initalize()
        {
            
        }
    }
}
