namespace Lunar.Core.Utilities.Data.Management
{
    public interface IDataLoader<out T> where T : IDataDescriptor
    {
        T Load(IDataLoaderArguments arguments);
    }
}
