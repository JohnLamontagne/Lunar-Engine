using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Server.Utilities.Data
{
    public class PlayerDataLoaderArguments : IDataLoaderArguments
    {
        public string Username { get; }

        public PlayerDataLoaderArguments(string username)
        {
            this.Username = username;
        }
    }
}
