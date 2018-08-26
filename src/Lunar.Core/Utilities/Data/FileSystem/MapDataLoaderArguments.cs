using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.Utilities.Data
{
    public class MapDataLoaderArguments : IDataManagerArguments
    {
        public string Name { get; }

        public MapDataLoaderArguments(string name)
        {
            this.Name = name;
        }
    }
}
