namespace Lunar.Core.World.Actor
{
    public class ClassInformation
    {
        public string Name { get; }

        public string TexturePath { get; }

        public Stats Stats { get; }

        public string StartMap { get; }

        public ClassInformation(string name, string texturePath, Stats stats, string startMap)
        {
            this.Name = name;
            this.TexturePath = texturePath;
            this.Stats = stats;
            this.StartMap = startMap;
        }
    }
}
