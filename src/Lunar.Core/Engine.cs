using System;
using System.IO;

namespace Lunar.Core
{
    public static class Engine
    {
        public static string ROOT_PATH { get; private set; }

        public static void Initialize(string rootPath)
        {
            Engine.ROOT_PATH = rootPath;
        }

        /// <summary>
        /// Walks up from the executable directory until a directory containing
        /// <paramref name="markerDirectory"/> is found. Used to resolve the
        /// project source root in dev mode regardless of build output depth.
        /// </summary>
        public static string FindDevRootPath(string markerDirectory)
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, markerDirectory)))
                dir = dir.Parent;
            return dir != null
                ? dir.FullName + Path.DirectorySeparatorChar
                : AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}