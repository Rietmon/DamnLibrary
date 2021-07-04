using System.Collections.Generic;
using Rietmon.Attributes;
using Rietmon.Behaviours;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace Rietmon.Game
{
    public class DontDestroy : UnityBehaviour
    {
        private static readonly List<short> ids = new List<short>();

        [SerializeField, ReadOnly] private short id;

        private void OnEnable()
        {
            if (ids.Contains(id))
            {
                DestroyObject();
                return;
            }

            DontDestroyOnLoad(gameObject);

            ids.Add(id);
        
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
