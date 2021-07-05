#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rietmon.Other
{
    [CustomPropertyDrawer(typeof(IRanged<>), true)]
    public class EditorRanged : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 18;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var minimalValue = new Rect(position.x, position.y, 100, 18);
            var dotLimiterRect = new Rect(position.x + 102, position.y, 100, 18);
            var maximalValue = new Rect(position.x + 113, position.y, 100, 18);

            EditorGUI.PropertyField(minimalValue, property.FindPropertyRelative("minimalValue"), GUIContent.none);
            ;
            EditorGUI.LabelField(dotLimiterRect, "-");
            EditorGUI.PropertyField(maximalValue, property.FindPropertyRelative("maximalValue"), GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
#endif
