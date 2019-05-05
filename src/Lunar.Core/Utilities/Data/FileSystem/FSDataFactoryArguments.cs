using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class FSDataFactoryArguments : IDataFactoryArguments
    {
        public string RootPath { get; }

        public FSDataFactoryArguments(string rootPath)
        {
            this.RootPath = rootPath;
        }
    }
}
