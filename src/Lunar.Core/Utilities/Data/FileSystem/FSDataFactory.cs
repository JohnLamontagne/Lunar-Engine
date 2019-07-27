/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class FSDataFactory : IDataManagerFactory
    {
        public T Create<T>(IDataFactoryArguments args) where T: IDataManager<IContentDescriptor>, new()
        {
            dynamic dataManager = new T();
            dataManager.RootPath = (args as FSDataFactoryArguments).RootPath;
            return dataManager;
        }

        public void Initalize()
        {
          
        }
    }
}
