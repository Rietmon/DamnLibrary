using System;
using System.Collections.Generic;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace DamnLibrary.Types.Pairs
{
    [Serializable]
    public struct Pair<T1, T2>
    {
        public T1 First
        {
            get => first;
            set => first = value;
        }

        public T2 Second
        {
            get => second;
            set => second = value;
        }
        
#if UNITY_5_3_OR_NEWER 
        [SerializeField] 
#endif
        private T1 first;

#if UNITY_5_3_OR_NEWER 
        [SerializeField] 
#endif
        private T2 second;

        public Pair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }
        
        public bool Equals(Pair<T1, T2> other) => 
            EqualityComparer<T1>.Default.Equals(first, other.first) 
            && EqualityComparer<T2>.Default.Equals(second, other.second);

        public override bool Equals(object obj) => 
            obj is Pair<T1, T2> other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(first, second);

        public static bool operator ==(Pair<T1, T2> l, Pair<T1, T2> r) => Equals(r.First, l.First) 
                                                                          && Equals(r.Second, l.Second);

        public static bool operator !=(Pair<T1, T2> l, Pair<T1, T2> r) => !(l == r);

        public static implicit operator KeyValuePair<T1, T2>(Pair<T1, T2> pair) =>
            new(pair.First, pair.Second);
        
        public static implicit operator Pair<T1, T2>(KeyValuePair<T1, T2> pair) =>
            new(pair.Key, pair.Value);
    }
}