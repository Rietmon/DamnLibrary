using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Prefab<T> where T : Object
{
    public T PrefabObject => prefabObject;

    public Transform DefaultParent
    {
        get => defaultParent;
        set => defaultParent = value;
    }
    
    [SerializeField] private T prefabObject;

    [SerializeField] private Transform defaultParent;

    public Prefab(T component)
    {
        prefabObject = component;
    }
    
    public T Instantiate() => 
        Instantiate(Vector3.zero, Quaternion.identity, Vector3.one, DefaultParent);

    public T Instantiate(Vector3 position) => 
        Instantiate(position, Quaternion.identity, Vector3.one, DefaultParent);

    public T Instantiate(Quaternion rotation) => 
        Instantiate(Vector3.zero, rotation, Vector3.one, DefaultParent);

    public T Instantiate(Transform parent) =>
        Instantiate(Vector3.zero, Quaternion.identity, Vector3.one, parent);
    
    public T Instantiate(Transform parent, Vector3 position) =>
        Instantiate(position, Quaternion.identity, Vector3.one, parent);

    public T Instantiate(Vector3 position, Quaternion rotation) =>
        Instantiate(position, rotation, Vector3.one, DefaultParent);

    public T Instantiate(Vector3 position, Vector3 scale) =>
        Instantiate(position, Quaternion.identity, scale, DefaultParent);

    public T Instantiate(Quaternion rotation, Vector3 scale) =>
        Instantiate(Vector3.zero, rotation, scale, DefaultParent);

    public T Instantiate(Vector3 position, Quaternion rotation, Vector3 scale) =>
        Instantiate(position, rotation, scale, DefaultParent);

    public T Instantiate(Vector3 position, Quaternion rotation, Vector3 scale, Transform parent)
    {
        var result = Object.Instantiate(prefabObject);
        var transform = result.GetTransform();
        
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
        transform.parent = parent;

        return result;
    }

    public static implicit operator Prefab<T>(T component) => new Prefab<T>(component);

    public static implicit operator bool(Prefab<T> prefab) => prefab != null;
}
