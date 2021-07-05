using System;
#if UNITY_2020
using UnityEngine;
#endif

namespace Rietmon.Other
{
    [Serializable]
    public class Pair<T1, T2>
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
        
        public Pair() { }

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }
}