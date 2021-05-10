using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SerializedPropertyExtensions
{
    public static T GetGeneralValue<T>(this SerializedProperty serializedProperty)
    {
        var targetObject = serializedProperty.serializedObject.targetObject;
        var targetObjectClassType = targetObject.GetType();
        var field = targetObjectClassType.GetField(serializedProperty.propertyPath);
        return (T)field.GetValue(targetObject);
    }
}
