using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Attempts to get the service by its specified type name
        /// </summary>
        /// <param name="serviceKTypeName"></param>
        /// <returns></returns>
        public IService GetService(string serviceKTypeName)
        {
            return _services.FirstOrDefault(s => s.Key.Name == serviceKTypeName).Value;
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
