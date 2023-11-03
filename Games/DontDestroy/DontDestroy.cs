#if UNITY_5_3_OR_NEWER 
using System.Collections.Generic;
using DamnLibrary.Attributes;
using DamnLibrary.Behaviours;
using DamnLibrary.Extensions;
using UnityEngine;

namespace DamnLibrary.Game
{
    public class DontDestroy : DamnBehaviour
    {
        private static List<short> Ids { get; } = new();

        [SerializeField, ReadOnly] private short id;

        private void OnEnable()
        {
            if (Ids.Contains(id))
            {
                DestroyObject();
                return;
            }

            DontDestroyOnLoad(gameObject);

            Ids.Add(id);
        
            RemoveComponent();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            id = RandomUtilities.RandomShort;
        }
#endif
    }
}
#endif