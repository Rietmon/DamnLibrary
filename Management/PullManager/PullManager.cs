using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rietmon.Game;
using UnityEngine;
using Object = UnityEngine.Object;

public class PullManager<T> : IDisposable where T : Object
{
    public int PullCapacity { get; }

    public int FreeObjects => objectsPull.Count;

    public int BusyObject => objectPullExecuting.Count;
    
    public Action<Object> OnWillBusy { get; set; }
    
    public Action<Object> OnWillFree { get; set; }

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
            };
            
            return obj;
        }
        else
        {
            var obj = CreateObject();

            endCallback += () =>
            {
                Object.Destroy(obj);
            };

            return obj;
        }
    }

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
    
    private T CreateObject() =>
        instantiateMethod != null ? instantiateMethod.Invoke(examplePrefab.PrefabObject) : examplePrefab.SimpleInstantiate();

    public void Dispose()
    {
        foreach (var obj in objectsPull)
            Object.Destroy(obj);
        
        foreach (var obj in objectPullExecuting)
            Object.Destroy(obj);
    }
}
