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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lunar.Server.Utilities
{
    /// <summary>
    /// Provides a dictionary for storage of world data which can be safely enumerated
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class WorldDictionary<K, T> : IEnumerable<T>
    {
        private readonly Dictionary<K, T> _dictionary;

        public WorldDictionary()
        {
            _dictionary = new Dictionary<K, T>();
        }

        public T this[K key] => _dictionary[key];


        public bool ContainsKey(K key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(K key, T value)
        {
            _dictionary.Add(key, value);
        }

        public void Remove(K key)
        {
            _dictionary.Remove(key);
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _dictionary.Values.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.Values.ToList().GetEnumerator();
        }
    }
}
