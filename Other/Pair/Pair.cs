using System;
using System.Collections.Generic;
#if UNITY_2020
using UnityEngine;
#endif

namespace Rietmon.Other
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
        
#if UNITY_2020
        [SerializeField] 
#endif
        private T1 first;

#if UNITY_2020
        [SerializeField] 
#endif
        private T2 second;

        public Pair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }

        public static implicit operator KeyValuePair<T1, T2>(Pair<T1, T2> pair) =>
            new KeyValuePair<T1, T2>(pair.First, pair.Second);
        
        public static implicit operator Pair<T1, T2>(KeyValuePair<T1, T2> pair) =>
            new Pair<T1, T2>(pair.Key, pair.Value);
    }
}