using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtensions
{
    public static Transform GetTransform(this Object obj)
    {
        switch (obj)
        {
            case Component component:
                return component.transform;
            case GameObject gameObject:
                return gameObject.transform;
            default:
                return null;
        }
    }
}
