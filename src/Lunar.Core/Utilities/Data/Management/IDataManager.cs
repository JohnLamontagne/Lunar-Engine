namespace Lunar.Core.Utilities.Data.Management
{
    public interface IDataManager<out T> where T : IDataDescriptor 
    {
        T Load(IDataManagerArguments arguments);

        void Save(IDataDescriptor descriptor);

        bool Exists(IDataManagerArguments arguments);
    }
}
