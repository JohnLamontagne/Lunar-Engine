using System.Reflection;
using System.Runtime.Loader;

namespace Lunar.Server.Scripting
{
    internal sealed class CollectibleScriptAlc : AssemblyLoadContext
    {
        public CollectibleScriptAlc(string name) : base(name, isCollectible: true) { }

        protected override Assembly Load(AssemblyName assemblyName) => null;
    }
}
