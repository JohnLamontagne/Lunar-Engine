using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class ContentFileDataLoaderArguments : IDataManagerArguments
    {
        public string FileName { get; }

        public ContentFileDataLoaderArguments(string fileName)
        {
            this.FileName = fileName;
        }
    }
}
