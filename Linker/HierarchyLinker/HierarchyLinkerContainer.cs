using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rietmon.Common.Linker
{
    [Serializable]
    public class HierarchyLinkerContainer<T>
    {
        public T Value => value;

        public HierarchyLinkerContainer<T> Parent => parent;

        public List<HierarchyLinkerContainer<T>> Childes => childes;
    
        [SerializeField] private T value;

        [SerializeField] private HierarchyLinkerContainer<T> parent;

        [SerializeField] private List<HierarchyLinkerContainer<T>> childes = new List<HierarchyLinkerContainer<T>>();

        public HierarchyLinkerContainer(T value)
        {
            this.value = value;
        }

        public void SetParent(HierarchyLinkerContainer<T> parent)
        {
            this.parent = parent;
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
