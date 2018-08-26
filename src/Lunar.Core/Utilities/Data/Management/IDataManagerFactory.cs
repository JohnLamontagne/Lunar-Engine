namespace Lunar.Core.Utilities.Data.Management
{
    public interface IDataManagerFactory : IService
    {
        T Create<T>() where T : IDataManager<IDataDescriptor>, new();
    }

}
