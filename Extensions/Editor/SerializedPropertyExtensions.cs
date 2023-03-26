#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DamnLibrary.Extensions
{
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Return the value of a SerializedProperty
        /// </summary>
        /// <param name="serializedProperty">Serialized property</param>
        /// <typeparam name="T">Value type</typeparam>
        /// <returns>Value</returns>
        public static T GetGeneralValue<T>(this SerializedProperty serializedProperty)
        {
            var targetObject = serializedProperty.serializedObject.targetObject;
            var targetObjectClassType = targetObject.GetType();
            var field = targetObjectClassType.GetField(serializedProperty.propertyPath);
            return (T)field.GetValue(targetObject);
        }
    }
}
#endif