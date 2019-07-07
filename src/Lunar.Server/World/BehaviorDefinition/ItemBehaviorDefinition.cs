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
using Lunar.Server.Utilities.Scripting;
using System;

namespace Lunar.Server.World.BehaviorDefinition
{
    public class ItemBehaviorDefinition
    {
        /// <summary>
        /// Invoked when the item is used
        /// </summary>
        public Action<ServerArgs> OnUse { get; set; }

        /// <summary>
        /// Invoked when the item is equipped
        /// </summary>
        public Action<ServerArgs> OnEquip { get; set; }

        /// <summary>
        /// Invoked when the item is acquired by an actor
        /// </summary>
        public Action<ServerArgs> OnAcquired { get; set; }

        /// <summary>
        /// Invoked when the item is dropped by an actor
        /// </summary>
        public Action<ServerArgs> OnDropped { get; set; }

        /// <summary>
        /// Invoked when the item is created within the gameworld
        /// </summary>
        public Action<ServerArgs> OnCreated { get; set; }
    }
}
