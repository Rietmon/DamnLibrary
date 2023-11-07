#if UNITY_5_3_OR_NEWER
using DamnLibrary.Games.Prefab;
using Object = UnityEngine.Object;

namespace DamnLibrary.Managements.Pools
{
    public class PoolManager<T> : IDisposable where T : Object
    {
        public int PoolCapacity { get; private set; }

        public int FreeObjectsCount => objectsPool.Count;

        public int BusyObjectsCount => objectPoolExecuting.Count;

        public Action<T> OnWillBusy { get; set; }

        public Action<T> OnWillFree { get; set; }

        private readonly Prefab<T> examplePrefab;

        private readonly Func<T, T> instantiateMethod;

        private readonly List<T> objectsPool = new();

        private readonly List<T> objectPoolExecuting = new();

        public PoolManager(int capacity, T example, Func<T, T> instantiateMethod = null)
        {
            PoolCapacity = capacity;
            examplePrefab = example;
            this.instantiateMethod = instantiateMethod;

            for (var i = 0; i < capacity; i++)
            {
                var pullObject = CreateObject();
                objectsPool.Add(pullObject);
            }
        }

        public void ForEachFreeObject(Action<T> action)
        {
            for (var i = 0; i < objectsPool.Count; i++)
            {
                var callback = ActionUtilities.GetDummy();
                var pullObject = GetObject(ref callback);
                action?.Invoke(pullObject);
                
                callback.Invoke();
            }
        }
        
        public async Task ForEachFreeObjectAsync(Func<T, Task> action)
        {
            for (var i = 0; i < objectsPool.Count; i++)
            {
                var callback = ActionUtilities.GetDummy();
                var pullObject = GetObject(ref callback);
                await action.Invoke(pullObject);
                
                callback.Invoke();
            }
        }

        public T GetObject(ref Action endCallback)
        {
            if (objectsPool.Count > 0)
            {
                var obj = objectsPool.First();
                objectsPool.RemoveAt(0);
                objectPoolExecuting.Add(obj);

                endCallback += () =>
                {
                    objectPoolExecuting.Remove(obj);
                    objectsPool.Add(obj);
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
                    obj.TotalDestroy();
                };

                OnWillBusy?.Invoke(obj);

                return obj;
            }
        }

        public bool IsObjectBusy(T pullObject) => objectPoolExecuting.Contains(pullObject);

        public void EnableGameObjectOptimization()
        {
            foreach (var freeObject in objectsPool)
                (freeObject as GameObject)?.SetActive(false);

            OnWillBusy += (obj) => (obj as GameObject)?.SetActive(true);
            OnWillFree += (obj) => (obj as GameObject)?.SetActive(false);
        }

        public void DisableGameObjectOptimization()
        {
            foreach (var freeObject in objectsPool)
                (freeObject as GameObject)?.SetActive(true);

            OnWillBusy -= (obj) => (obj as GameObject)?.SetActive(true);
            OnWillFree -= (obj) => (obj as GameObject)?.SetActive(false);
        }

        public void DestroyLastObjects(int count, bool onlyFree = false) =>
            DestroyObjects(objectsPool.Count - 1, 0, count, onlyFree);
        
        public void DestroyFirstObjects(int count, bool onlyFree = false) =>
            DestroyObjects(0, objectsPool.Count - 1, count, onlyFree);

        public void DestroyObjects(int fromIndex, int toIndex, int count, bool onlyFree = false)
        {
            for (var i = fromIndex; i != toIndex; i += fromIndex < toIndex ? 1 : -1)
            {
                if (count == 0)
                    return;
                
                var poolObject = objectsPool[i];
                if (onlyFree && IsObjectBusy(poolObject))
                    continue;

                DestroyObject(i);
                count--;
            }
        }

        public void DestroyObject(int index)
        {
            if (objectsPool.Count <= index)
                return;

            var poolObject = objectsPool[index];

            objectsPool.Remove(poolObject);
            objectPoolExecuting.Remove(poolObject);
            OnWillFree?.Invoke(poolObject);
            
            poolObject.TotalDestroy();
            
            PoolCapacity--;
        }

        private T CreateObject() =>
            instantiateMethod != null
                ? instantiateMethod.Invoke(examplePrefab.PrefabObject)
                : examplePrefab.Instantiate();

        public void Dispose()
        {
            foreach (var obj in objectsPool)
                obj.TotalDestroy();

            foreach (var obj in objectPoolExecuting)
                obj.TotalDestroy();
        }
    }
}
#endif