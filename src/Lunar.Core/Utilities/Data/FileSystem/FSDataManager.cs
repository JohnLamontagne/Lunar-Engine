using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public abstract class FSDataManager<T> : IDataManager<T> where T : IContentDescriptor
    {
        public string RootPath { get; set; }

        public abstract bool Exists(IDataManagerArguments arguments);

        public abstract T Load(IDataManagerArguments arguments);

        public abstract void Save(IContentDescriptor descriptor, IDataManagerArguments arguments);
    }
}