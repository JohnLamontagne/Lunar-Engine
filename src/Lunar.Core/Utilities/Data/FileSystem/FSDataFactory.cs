using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class FSDataFactory : IDataManagerFactory
    {
        public T Create<T>() where T : IDataManager<IDataDescriptor>, new()
        {
            return new T();
        }

        public void Initalize()
        {
          
        }
    }
}
