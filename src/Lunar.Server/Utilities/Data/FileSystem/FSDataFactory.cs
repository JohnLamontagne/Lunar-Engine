using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Server.Utilities.Data.SQL
{
    public class FSDataFactory : IDataLoaderFactory
    {
        public T Create<T>() where T : IDataLoader<IDataDescriptor>, new()
        {
            return new T();
        }

        public void Initalize()
        {
          
        }
    }
}
