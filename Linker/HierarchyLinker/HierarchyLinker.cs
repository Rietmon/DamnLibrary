﻿using System;
using UnityEngine;

namespace Rietmon.Linker
{
    [Serializable]
    public class HierarchyLinker<T>
    {
        public HierarchyLinkerContainer<T> Entry => entry;

        public T Current => current.Value;

        [SerializeField] private HierarchyLinkerContainer<T> entry;

        private HierarchyLinkerContainer<T> current;
    
        public HierarchyLinker(T entry)
        {
            this.entry = new HierarchyLinkerContainer<T>(entry);
        }
    
        public virtual bool GoDown(int index)
        {
            if (index >= current.Childes.Count)
                return false;

            current = current.Childes[index];

            return true;
        }

        public virtual bool GoUp()
        {
            if (current.Parent == null)
                return false;

            current = current.Parent;
            return true;
        }

        public virtual void AddChild(T child)
        {
            current.AddChild(child);
        }
    }
}
