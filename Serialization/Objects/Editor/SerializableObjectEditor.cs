#if ENABLE_SERIALIZATION && UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using Rietmon.Serialization;
using UnityEditor;
using UnityEngine;

namespace Rietmon.Serialization
{
    [CustomEditor(typeof(SerializableObject)), CanEditMultipleObjects]
    public class SerializableObjectEditor : Editor
    {
        private SerializedProperty serializeAllComponentsProperty;
        private SerializedProperty serializableComponentsProperty;
        private SerializedProperty scriptProperty;

        private void OnEnable()
        {
            scriptProperty = serializedObject.FindProperty("m_Script");
            serializeAllComponentsProperty = serializedObject.FindProperty("serializeAllComponents");
            serializableComponentsProperty = serializedObject.FindProperty("serializableComponents");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUI.enabled = false;
            EditorGUILayout.PropertyField(scriptProperty, true);
            GUI.enabled = true;
            EditorGUILayout.PropertyField(serializeAllComponentsProperty);
            if (!serializeAllComponentsProperty.boolValue)
                EditorGUILayout.PropertyField(serializableComponentsProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif