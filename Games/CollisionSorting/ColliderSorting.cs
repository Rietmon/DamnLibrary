#if UNITY_5_3_OR_NEWER && !DISABLE_PHYSICS
using System.Collections.Generic;
using DamnLibrary.Behaviours;
using UnityEngine;

namespace DamnLibrary.Games
{
    [RequireComponent(typeof(Collider2D))]
    public class ColliderSorting : DamnBehaviour
    {
        /// <summary>
        /// Collider sorting layer
        /// </summary>
        public virtual int SortingLayer
        {
            get => sortingLayer;
            set => sortingLayer = value;
        }

        /// <summary>
        /// Attached collider
        /// </summary>
        public Collider2D Collider => collider;
    
        [SerializeField] private int sortingLayer;

        [SerializeField] private new Collider2D collider;

        private void Reset()
        {
            collider = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Return greater collider from array of colliders
        /// </summary>
        /// <param name="colliders"></param>
        /// <returns></returns>
        public static Collider2D GetGreaterCollider(params Collider2D[] colliders)
        {
            var collidersSorting = new List<ColliderSorting>();
            foreach (var collider in colliders)
            {
                if (collider == null)
                    continue;
            
                if (collider.TryGetComponent<ColliderSorting>(out var colliderSorting))
                    collidersSorting.Add(colliderSorting);
            }

            switch (collidersSorting.Count)
            {
                case 0:
                    return null;
                case 1:
                    return collidersSorting[0].Collider;
            }

            var greaterColliderIndex = 0;
            for (var i = 0; i < collidersSorting.Count; i++)
            {
                if (collidersSorting[i].SortingLayer >= collidersSorting[greaterColliderIndex].SortingLayer)
                    greaterColliderIndex = i;
            }

            return collidersSorting[greaterColliderIndex].Collider;
        }
    }
}
#endif