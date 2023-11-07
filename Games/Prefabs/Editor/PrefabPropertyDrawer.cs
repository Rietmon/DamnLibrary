#if UNITY_EDITOR
using DamnLibrary.Games;
using UnityEditor;
using UnityEngine;

namespace DamnLibrary.Game
{
    [CustomPropertyDrawer(typeof(Prefab<>))]
    public class PrefabPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var prefabRect = new Rect(position.x, position.y, position.width, position.height);

            EditorGUI.PropertyField(prefabRect, property.FindPropertyRelative("prefabObject"), GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
#endif