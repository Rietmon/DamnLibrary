using System.Collections.Generic;
using Rietmon.Attributes;
using Rietmon.Behaviours;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Rietmon.Game
{
    public class DontDestroy : UnityBehaviour
    {
        private static readonly List<int> ids = new List<int>();

        [SerializeField, ReadOnly] private int id;

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
            id = EditorPrefs.GetInt("Editor_LastDontDestroyId", 1);

            EditorPrefs.SetInt("Editor_LastDontDestroyId", id + 1);
        }
#endif
    }
}
