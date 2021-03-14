using System;
using System.Collections;
using Rietmon.Behaviours;
using UnityEngine;

namespace Rietmon.Async
{
    public class AsyncExecutor : UnityBehaviour
    {
        public static AsyncExecutor Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameObject("[AsyncExecutor]").AddComponent<AsyncExecutor>();

                return instance;
            }
        }

        private static AsyncExecutor instance;
    
        private void Awake()
        {
            instance = this;
        }

        public static void Handle(Action action) => Instance.StartCoroutine(HandleAsync(action));
    
        private static IEnumerator HandleAsync(Action action)
        {
            action?.Invoke();
        
            yield break;
        }
    }
}
