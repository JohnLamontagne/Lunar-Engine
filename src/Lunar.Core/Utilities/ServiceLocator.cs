/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
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
