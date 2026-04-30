namespace Lunar.Core
{
    public static class Engine
    {
        public static string ROOT_PATH { get; private set; }

        public static void Initialize(string rootPath)
        {
            Engine.ROOT_PATH = rootPath;
        }
    }
}