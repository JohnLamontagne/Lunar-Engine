 using System;

namespace Lunar.Server.Utilities.Scripting
{
    public static class ScriptUtilities
    {
        public static Type GetTypeOf(string typeName)
        {
            return Type.GetType(typeName);
        }
    }
}
