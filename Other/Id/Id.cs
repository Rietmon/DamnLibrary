// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Reflection;
// using JetBrains.Annotations;
// #if UNITY_EDITOR
// using UnityEditor;
// #endif
// using UnityEngine;
//
// [Serializable]
// public class Id : ISerializationCallbackReceiver
// {
//     private Internal_Id UsedId => 
//         id8 != null ? 
//             id8 : id16 != null ? 
//                 id16 : id32 != null ? 
//                     id32 : id64 != null ? 
//                         id64 : id128 != null ? 
//                             (Internal_Id)id128 : null;
//
//     [SerializeField] private Internal_Id8 id8;
//     [SerializeField] private Internal_Id16 id16;
//     [SerializeField] private Internal_Id32 id32;
//     [SerializeField] private Internal_Id64 id64;
//     [SerializeField] private Internal_Id128 id128;
//
//     public static Id Create8(byte value) =>
//         new Id
//         {
//             id8 = new Internal_Id8(value)
//         };
//     
//     public static Id Create8() =>
//         Create8(RandomUtilities.RandomByte);
//     
//     public static Id Create16(short value) =>
//         new Id
//         {
//             id16 = new Internal_Id16(value)
//         };
//     
//     public static Id Create16() =>
//         Create16(RandomUtilities.RandomShort);
//
//     public static Id Create32(int value) =>
//         new Id
//         {
//             id32 = new Internal_Id32(value)
//         };
//     
//     public static Id Create32() =>
//         Create32(RandomUtilities.RandomInt);
//     
//     public static Id Create64(long value) =>
//         new Id
//         {
//             id64 = new Internal_Id64(value)
//         };
//     
//     public static Id Create64() =>
//         Create64(RandomUtilities.RandomLong);
//     
//     public static Id Create128(decimal value) =>
//         new Id
//         {
//             id128 = new Internal_Id128(value)
//         };
//     
//     public static Id Create128() =>
//         Create128(RandomUtilities.RandomDecimal);
//
//     public static bool operator ==(Id first, Id second)
//     {
//         if (Equals(first, null) && Equals(second, null)) return true;
//         if (Equals(first, null)) return false;
//         if (Equals(second, null)) return false;
//         return first.UsedId == second.UsedId;
//     }
//
//     public static bool operator !=(Id first, Id second)
//     {
//         return !(first == second);
//     }
//
//     public void OnBeforeSerialize() { }
//
//     public void OnAfterDeserialize()
//     {
//         if (id8.size == 0)
//             id8 = null;
//         if (id16.size == 0)
//             id16 = null;
//         if (id32.size == 0)
//             id32 = null;
//         if (id64.size == 0)
//             id64 = null;
//         if (id128.size == 0)
//             id128 = null;
//     }
//
//     [CustomPropertyDrawer(typeof(Id))]
//     public class IdPropertyDrawer : PropertyDrawer
//     {
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             EditorGUI.BeginProperty(position, label, property);
//
//             var indent = EditorGUI.indentLevel;
//             EditorGUI.indentLevel = 0;
//
//             position.height = 14;
//
//             EditorGUI.PropertyField(position, property);
//
//             EditorGUI.indentLevel = indent;
//
//             EditorGUI.EndProperty();
//         }
//     }
//
//     [Serializable]
//     private abstract class Internal_Id
//     {
//         public readonly byte size;
//         
//         public abstract object Id { get; set; }
//
//         public Internal_Id(object value, byte valueSize)
//         {
//             Id = value;
//             size = valueSize;
//         }
//
//         public abstract bool CompareWithOther(Internal_Id other);
//         
//         public static bool operator ==(Internal_Id first, Internal_Id second)
//         {
//             if (Equals(first, null) && Equals(second, null)) return true;
//             if (Equals(first, null)) return false;
//             if (Equals(second, null)) return false;
//
//             var maximalValue = first.size > second.size ? first : second;
//             var otherValue = first.size > second.size ? second : first;
//
//             return maximalValue.CompareWithOther(otherValue);
//         }
//
//         public static bool operator !=(Internal_Id first, Internal_Id second)
//         {
//             return !(first == second);
//         }
//     }
//     
//     [Serializable]
//     private class Internal_Id8 : Internal_Id
//     {
//         public override object Id
//         {
//             get => value;
//             set => this.value = (byte)value;
//         }
//
//         [SerializeField] private byte value;
//         
//         public Internal_Id8(byte value) : base(value, 1) { }
//
//         public override bool CompareWithOther(Internal_Id other)
//         {
//             return (byte)Id == (byte)other.Id;
//         } 
//     }
//     
//     [Serializable]
//     private class Internal_Id16 : Internal_Id
//     {
//         public override object Id
//         {
//             get => value;
//             set => this.value = (short)value;
//         }
//
//         [SerializeField] private short value;
//         
//         public Internal_Id16(short value) : base(value, 2) { }
//
//         public override bool CompareWithOther(Internal_Id other)
//         {
//             if (other.Id is byte byteValue)
//                 return (short)Id == byteValue;
//             
//             return (short)Id == (short)other.Id;
//         }
//     }
//     
//     [Serializable]
//     private class Internal_Id32 : Internal_Id
//     {
//         public override object Id
//         {
//             get => value;
//             set => this.value = (int)value;
//         }
//
//         [SerializeField] private int value;
//         
//         public Internal_Id32(int value) : base(value, 4) { }
//
//         public override bool CompareWithOther(Internal_Id other)
//         {
//             if (other.Id is byte byteValue)
//                 return (int)Id == byteValue;
//             if (other.Id is short shortValue)
//                 return (int)Id == shortValue;
//             
//             return (int)Id == (int)other.Id;
//         }
//     }
//
//     [Serializable]
//     private class Internal_Id64 : Internal_Id
//     {
//         public override object Id
//         {
//             get => value;
//             set => this.value = (long)value;
//         }
//
//         [SerializeField] private long value;
//         
//         public Internal_Id64(long value) : base(value, 8) { }
//
//         public override bool CompareWithOther(Internal_Id other)
//         {
//             if (other.Id is byte byteValue)
//                 return (long)Id == byteValue;
//             if (other.Id is short shortValue)
//                 return (long)Id == shortValue;
//             if (other.Id is int intValue)
//                 return (long)Id == intValue;
//             
//             return (long)Id == (long)other.Id;
//         }
//     }
//     
//     [Serializable]
//     private class Internal_Id128 : Internal_Id
//     {
//         public override object Id
//         {
//             get => value;
//             set => this.value = (decimal)value;
//         }
//
//         [SerializeField] private decimal value;
//         
//         public Internal_Id128(decimal value) : base(value, 16) { }
//
//         public override bool CompareWithOther(Internal_Id other)
//         {
//             if (other.Id is byte byteValue)
//                 return (decimal)Id == byteValue;
//             if (other.Id is short shortValue)
//                 return (decimal)Id == shortValue;
//             if (other.Id is int intValue)
//                 return (decimal)Id == intValue;
//             if (other.Id is long longValue)
//                 return (decimal)Id == longValue;
//             
//             return (long)Id == (decimal)other.Id;
//         }
//     }
//     
//     [CustomPropertyDrawer(typeof(Internal_Id), true)]
//     public abstract class Internal_IdPropertyDrawer : PropertyDrawer
//     {
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             EditorGUI.BeginProperty(position, label, property);
//
//             var indent = EditorGUI.indentLevel;
//             EditorGUI.indentLevel = 0;
//             
//             var id = ((SerializableUnityBehaviour)property.serializedObject.targetObject).SerializableId.UsedId;
//             if (id == null || id.Id == null)
//                 return;
//             var value = id.Id;
//             switch (value)
//             {
//                 case byte bValue:
//                     EditorGUI.LabelField(position, bValue.ToString());
//                     break;
//                 case short sValue:
//                     EditorGUI.LabelField(position, sValue.ToString());
//                     break;
//                 case int iValue:
//                     EditorGUI.LabelField(position, iValue.ToString());
//                     break;
//                 case long lValue:
//                     EditorGUI.LabelField(position, lValue.ToString());
//                     break;
//                 case decimal dValue:
//                     EditorGUI.LabelField(position, dValue.ToString());
//                     break;
//             }
//
//             EditorGUI.indentLevel = indent;
//
//             EditorGUI.EndProperty();
//         }
//     }
// }
