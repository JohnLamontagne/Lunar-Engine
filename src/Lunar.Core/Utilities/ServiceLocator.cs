using System;
using System.Collections.Generic;

namespace Lunar.Core.Utilities
{
    public class ServiceLocator
    {
        /// <summary>
        /// We need to maintain this as NLua cannot access generic methods... bummer
        /// </summary>
        private Dictionary<Type, IService> _services;

        public ServiceLocator()
        {
            _services = new Dictionary<Type, IService>();
        }

        public IService GetService(Type serviceType)
        {
            return _services[serviceType];
        }

        public T GetService<T>() where T : IService
        {
            return (T)(_services[typeof(T)]);
        }

        public void RegisterService(IService service)
        {
            _services.Add(service.GetType(), service);
        }
    }
}
