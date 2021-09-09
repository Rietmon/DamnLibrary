#if UNITY_5_3_OR_NEWER 
using System;
using System.Collections.Generic;
using System.Linq;
#if ENABLE_UNI_TASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif
using Rietmon.Extensions;
using Rietmon.Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rietmon.Management
{
    public class PullManager<T> : IDisposable where T : Object
    {
        public int PullCapacity { get; private set; }

        public int FreeObjectsCount => objectsPull.Count;

        public int BusyObjectsCount => objectPullExecuting.Count;

        public Action<T> OnWillBusy { get; set; }

        public Action<T> OnWillFree { get; set; }

        private readonly Prefab<T> examplePrefab;

        private readonly Func<T, T> instantiateMethod;

        private readonly List<T> objectsPull = new List<T>();

        private readonly List<T> objectPullExecuting = new List<T>();

        public PullManager(int capacity, T example, Func<T, T> instantiateMethod = null)
        {
            PullCapacity = capacity;
            examplePrefab = example;
            this.instantiateMethod = instantiateMethod;

            for (var i = 0; i < capacity; i++)
            {
                var pullObject = CreateObject();
                objectsPull.Add(pullObject);
            }
        }

        public void ForEachFreeObject(Action<T> action)
        {
            for (var i = 0; i < objectsPull.Count; i++)
            {
                var callback = ActionUtilities.GetDummy();
                var pullObject = GetObject(ref callback);
                action?.Invoke(pullObject);
                
                callback.Invoke();
            }
        }
        
#if ENABLE_UNI_TASK
        public async UniTask ForEachFreeObjectAsync(Func<T, UniTask> action)
#else
        public async Task ForEachFreeObjectAsync(Func<T, Task> action)
#endif
        {
            for (var i = 0; i < objectsPull.Count; i++)
            {
                var callback = ActionUtilities.GetDummy();
                var pullObject = GetObject(ref callback);
                await action.Invoke(pullObject);
                
                callback.Invoke();
            }
        }

        public T GetObject(ref Action endCallback)
        {
            if (objectsPull.Count > 0)
            {
                var obj = objectsPull.First();
                objectsPull.RemoveAt(0);
                objectPullExecuting.Add(obj);

                endCallback += () =>
                {
                    objectPullExecuting.Remove(obj);
                    objectsPull.Add(obj);
                    OnWillFree?.Invoke(obj);
                };

                OnWillBusy?.Invoke(obj);

                return obj;
            }
            else
            {
                var obj = CreateObject();

                endCallback += () =>
                {
                    OnWillFree?.Invoke(obj);
                    Object.Destroy(obj);
                };

                OnWillBusy?.Invoke(obj);

                return obj;
            }
        }

        public bool IsObjectBusy(T pullObject) => objectPullExecuting.Contains(pullObject);

        public void EnableGameObjectOptimization()
        {
            foreach (var freeObject in objectsPull)
                (freeObject as GameObject)?.SetActive(false);

            OnWillBusy += (obj) => (obj as GameObject)?.SetActive(true);
            OnWillFree += (obj) => (obj as GameObject)?.SetActive(false);
        }

        public void DisableGameObjectOptimization()
        {
            foreach (var freeObject in objectsPull)
                (freeObject as GameObject)?.SetActive(true);

            OnWillBusy -= (obj) => (obj as GameObject)?.SetActive(true);
            OnWillFree -= (obj) => (obj as GameObject)?.SetActive(false);
        }

        public void DestroyLastObjects(int count, bool onlyFree = false) =>
            DestroyObjects(objectsPull.Count - 1, 0, count, onlyFree);
        
        public void DestroyFirstObjects(int count, bool onlyFree = false) =>
            DestroyObjects(0, objectsPull.Count - 1, count, onlyFree);

        public void DestroyObjects(int fromIndex, int toIndex, int count, bool onlyFree = false)
        {
            for (var i = fromIndex; i != toIndex; i += fromIndex < toIndex ? 1 : -1)
            {
                if (count == 0)
                    return;
                
                var pullObject = objectsPull[i];
                if (onlyFree && IsObjectBusy(pullObject))
                    continue;

                DestroyObject(i);
                count--;
            }
        }

        public void DestroyObject(int index)
        {
            if (objectsPull.Count <= index)
                return;

            var pullObject = objectsPull[index];

            objectsPull.Remove(pullObject);
            objectPullExecuting.Remove(pullObject);
            OnWillFree?.Invoke(pullObject);
            
            pullObject.TotalDestroy();
            
            PullCapacity--;
        }

        private T CreateObject() =>
            instantiateMethod != null
                ? instantiateMethod.Invoke(examplePrefab.PrefabObject)
                : examplePrefab.SimpleInstantiate();

        public void Dispose()
        {
            foreach (var obj in objectsPull)
                Object.Destroy(obj);

            foreach (var obj in objectPullExecuting)
                Object.Destroy(obj);
        }
    }
}
#endif