#if UNITY_2020
using System.Collections.Generic;
using System.Linq;
using Rietmon.Behaviours;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ColliderSorting : UnityBehaviour
{
    public CollisionSortingLayers SortingLayer
    {
        get => sortingLayer;
        set => sortingLayer = value;
    }

    public Collider2D Collider => collider;
    
    [SerializeField] private CollisionSortingLayers sortingLayer;

    [SerializeField] private Collider2D collider;

    private void Reset()
    {
        collider = GetComponent<Collider2D>();
    }

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

        if (collidersSorting.Count == 0)
            return colliders.FirstOrDefault();

        if (collidersSorting.Count == 1)
            return collidersSorting[0].Collider;

        var greaterColliderIndex = 0;
        for (var i = 0; i < collidersSorting.Count; i++)
        {
            if (collidersSorting[i].SortingLayer >= collidersSorting[greaterColliderIndex].SortingLayer)
                greaterColliderIndex = i;
        }

        return collidersSorting[greaterColliderIndex].Collider;
    }

    public enum CollisionSortingLayers : byte
    {
        Default = 0,
        NonCollisionStatic = 50,
        CollisionStatic = 100,
        LevelChanger = 255,
    }
}
#endif