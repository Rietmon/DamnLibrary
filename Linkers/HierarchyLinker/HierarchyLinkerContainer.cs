using System;
using System.Collections.Generic;
#if UNITY_5_3_OR_NEWER 
using UnityEngine;
#endif

namespace Rietmon.Linkers
{
    [Serializable]
    public class HierarchyLinkerContainer<T>
    {
        public T Value => value;

        public HierarchyLinkerContainer<T> Parent => parent;

        public List<HierarchyLinkerContainer<T>> Childes => childes;
        
#if UNITY_5_3_OR_NEWER 
        [SerializeField] 
#endif
        private T value;

#if UNITY_5_3_OR_NEWER 
        [SerializeField] 
#endif
        private HierarchyLinkerContainer<T> parent;

#if UNITY_5_3_OR_NEWER 
        [SerializeField] 
#endif
        private List<HierarchyLinkerContainer<T>> childes = new List<HierarchyLinkerContainer<T>>();

        public HierarchyLinkerContainer(T value)
        {
            this.value = value;
        }

        public void SetParent(HierarchyLinkerContainer<T> newParent)
        {
            parent.childes.Remove(this);
            newParent.childes.Add(this);
            parent = newParent;
        }

        public void AddChild(T child)
        {
            var childContainer = new HierarchyLinkerContainer<T>(child);
            childes.Add(childContainer);
        }

        public void RemoveChild(HierarchyLinkerContainer<T> child)
        {
            childes.Remove(child);
        }
    }
}
