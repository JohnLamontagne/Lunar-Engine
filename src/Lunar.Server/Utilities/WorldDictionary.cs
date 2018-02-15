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
