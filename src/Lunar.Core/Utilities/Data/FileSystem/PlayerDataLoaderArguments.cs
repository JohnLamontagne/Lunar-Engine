using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class PlayerDataLoaderArguments : IDataManagerArguments
    {
        public string Username { get; }

        public PlayerDataLoaderArguments(string username)
        {
            this.Username = username;
        }
    }
}
