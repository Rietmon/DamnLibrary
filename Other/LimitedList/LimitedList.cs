using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Rietmon.Other
{
    public class LimitedList<T> : 
        IList<T>
    {
        public int Count => list.Count;

        public bool IsReadOnly => false;

        public int Limit { get; set; } = 0xfffffff;
        
        private readonly List<T> list = new List<T>();
        
        public LimitedList()
        {
        }

        public LimitedList(int limit)
        {
            Limit = limit;
        }

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

        public T this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public void Add(T item) => Internal_Add(item);

        public void Clear() => list.Clear();

        public bool Contains(T item) => list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public bool Remove(T item) => list.Remove(item);

        public int IndexOf(T item) => list.IndexOf(item);

        public void Insert(int index, T item) => list.Insert(index, item);

        public void RemoveAt(int index) => list.RemoveAt(index);

        private void Internal_Add(T item)
        {
            Add(item);
            if (Count > Limit)
                RemoveAt(0);
        }
    }
}