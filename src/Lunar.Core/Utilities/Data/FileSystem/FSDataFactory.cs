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
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Core.World.Structure;
using System;
using System.Collections.Generic;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class FSDataFactory : IDataManagerFactory
    {
        private Dictionary<Type, Type> _lookupTable;

        public FSDataFactory()
        {
            _lookupTable = new Dictionary<Type, Type>();
        }

        public IDataManager<T> Create<T>(IDataFactoryArguments args) where T: IContentDescriptor
        {
            if (_lookupTable.ContainsKey(typeof(T)))
            {
                var dataManager = (FSDataManager<T>)Activator.CreateInstance(_lookupTable[typeof(T)]);
                dataManager.RootPath = (args as FSDataFactoryArguments).RootPath;
                return (IDataManager<T>)dataManager;
            }

            return null;
        }

        public void Initalize()
        {
            _lookupTable.Add(typeof(MapDescriptor<LayerDescriptor<TileDescriptor<SpriteInfo>>>), typeof(MapFSDataManager));
            _lookupTable.Add(typeof(BaseAnimation<IAnimationLayer<SpriteInfo>>), typeof(AnimationFSDataManager));
            _lookupTable.Add(typeof(PlayerDescriptor), typeof(PlayerFSDataManager));
            _lookupTable.Add(typeof(NPCDescriptor), typeof(NPCFSDataManager));
        }
    }
}
