namespace Lunar.Core.Utilities.Data.Management
{
    public interface IDataLoaderFactory : IService
    {
        T Create<T>() where T : IDataLoader<IDataDescriptor>, new();
    }

}
