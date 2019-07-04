using System.Collections;
using System.Collections.Generic;

namespace Lunar.Core.Utilities.Data
{
    /// <summary>
    /// A collection which functions as a stack with the added
    /// property of being able to remove specific items or items at specific positions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FlexibleStack<T> : IEnumerable<T>
    {
        private List<T> _items;

        /// <summary>
        /// An ordered collection of all the items in the stack.
        /// </summary>
        public ICollection<T> Values => _items;

        public int Count => _items.Count;

        public T this[int index]
        {
            get { return _items[index]; }
            set { _items[index] = value; }
        }
        
        public FlexibleStack()
        {
            _items = new List<T>();
        }

        public FlexibleStack(int capacity)
        {
            _items = new List<T>(capacity);
        }

        /// <summary>
        /// Inserts the item at the top of the stack.
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            _items.Insert(0, item);
        }

        public void Push(ICollection<T> items)
        {
            foreach (var item in items)
                this.Push(item);
        }

        /// <summary>
        /// Inserts the item at the bottom of the stack.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Add(ICollection<T> items)
        {
            foreach (var item in items)
                this.Add(item);
        }


        /// <summary>
        /// Returns the item at the top of the stack.
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            if (_items.Count > 0)
            {
                T val = _items[0];
                _items.RemoveAt(0);
                return val;
            }
            else
                return default(T);
        }

        /// <summary>
        /// Returns the item at the top of the stack without removing it.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (_items.Count > 0)
                return _items[0];
            else
                return default(T);
        }

        /// <summary>
        /// Removes the specified item from the stack.
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            _items.Remove(item);
        }

        /// <summary>
        /// Removes the item at the position relative to the top of the stack (from 0 onward).
        /// </summary>
        /// <param name="pos"></param>
        public void RemoveAt(int pos)
        {
            _items.RemoveAt(pos);
        }

        /// <summary>
        /// Removes all items from the stack
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }
    }
}
