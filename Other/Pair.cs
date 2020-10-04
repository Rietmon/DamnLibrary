using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Rietmon.Common
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

        [SerializeField] private T1 first;

        [SerializeField] private T2 second;
        
        public Pair() { }

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }
}