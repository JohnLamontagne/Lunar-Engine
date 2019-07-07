using Lunar.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Core
{
    public static class Engine
    {
        public static ServiceLocator Services { get; private set; }

        public static void Initialize()
        {
            Services = new ServiceLocator();
            Services.Register(new Logger());
        }
    }
}