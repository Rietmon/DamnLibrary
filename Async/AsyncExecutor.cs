using System;
using System.Collections;
using Rietmon.Behaviours;

namespace Rietmon.Async
{
    public class AsyncExecutor : UnityBehaviour
    {
        private static AsyncExecutor instance;
    
        private void Awake()
        {
            instance = this;
        }

        public static void Handle(Action action) => instance.StartCoroutine(HandleAsync(action));
    
        private static IEnumerator HandleAsync(Action action)
        {
            action?.Invoke();
        
            yield break;
        }
    }
}
