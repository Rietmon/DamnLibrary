using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rietmon.Extensions;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct Id
{
    private Internal_Id InternalId
    {
        get
        {
            if (internalId.Type == 0)
            {
                var bytes = internalId.valueBytes;
                internalId = internalId.valueBytes.Length switch
                {
                    1 => new Internal_Id8 {valueBytes = bytes},
                    2 => new Internal_Id16 {valueBytes = bytes},
                    _ => internalId
                };
            }

            return internalId;
        }
    }
    
    [SerializeField] private Internal_Id internalId;

    public static Id Create8(byte value) => new Id
    {
        internalId = new Internal_Id8
        {
            valueBytes = new[] {value}
        }
    };

    public static Id Create16(short value) => new Id
    {
        internalId = new Internal_Id16
        {
            valueBytes = BitConverter.GetBytes(value)
        }
    };

    public static bool operator ==(Id first, Id second)
    {
        var isFirstNull = Equals(first, null);
        var isSecondNull = Equals(second, null);

        if (isFirstNull && isSecondNull)
            return true;

        if (isFirstNull != isSecondNull)
            return false;

        return first.internalId == second.internalId;
    }

    public static bool operator !=(Id first, Id second) => !(first == second);
    
    [Serializable]
    private class Internal_Id
    {
        public virtual byte Type => 0;
        
        public byte[] valueBytes;

        protected virtual bool CompareWith(Internal_Id other) => false;
        
        public static bool operator ==(Internal_Id first, Internal_Id second)
        {
            var preCompare = CompareUtilities.PreCompare(first, second);
            if (preCompare != null)
                return preCompare.Value;

            var greaterSizeValue = first.valueBytes.Length > second.valueBytes.Length ? first : second;
            var smallerSizeValue = first.valueBytes.Length > second.valueBytes.Length ? second : first;
            return greaterSizeValue.CompareWith(smallerSizeValue);
        }

        public static bool operator !=(Internal_Id first, Internal_Id second) => !(first == second);
    }

    [Serializable]
    private class Internal_Id8 : Internal_Id
    {
        public override byte Type => 1;
        
        public byte Value => value ?? (byte)(value = valueBytes.First());
        
        private byte? value;

        protected override bool CompareWith(Internal_Id other)
        {
            if (other is Internal_Id8 id8)
                return Value == id8.Value;
            
            return false;
        }
    }

    [Serializable]
    private class Internal_Id16 : Internal_Id
    {
        public override byte Type => 2;
        
        public short Value => value ?? (short)(value = BitConverter.ToInt16(valueBytes, 0));
        
        private short? value;
        
        protected override bool CompareWith(Internal_Id other)
        {
            if (other is Internal_Id8 id8)
                return Value == id8.Value;
            
            if (other is Internal_Id16 id16)
                return Value == id16.Value;
            
            return false;
        }
    }

    [CustomPropertyDrawer(typeof(Id), true)]
    private class Internal_IdPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property, label, true);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        { 
            GUI.enabled = false;
            var internalId = (Id)property.
                serializedObject.
                targetObject.
                GetType().
                GetField(property.propertyPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).
                GetValue(property.serializedObject.targetObject);
            var labelValue = internalId.InternalId switch
            {
                Internal_Id8 id8 => id8.Value.ToString(),
                Internal_Id16 id16 => id16.Value.ToString(),
                _ => "null"
            };

            EditorGUI.LabelField(position, label.text, labelValue);
            GUI.enabled = true;
        }
    }
}
