using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DamnLibrary.Serializations.Serializables;

namespace DamnLibrary.Serializations
{
    public partial class SerializationStream
    {
        private static readonly Type boolType = typeof(bool);
        private static readonly Type sbyteType = typeof(sbyte);
        private static readonly Type byteType = typeof(byte);
        private static readonly Type shortType = typeof(short);
        private static readonly Type ushortType = typeof(ushort);
        private static readonly Type intType = typeof(int);
        private static readonly Type uintType = typeof(uint);
        private static readonly Type longType = typeof(long);
        private static readonly Type ulongType = typeof(ulong);
        private static readonly Type charType = typeof(char);
        private static readonly Type floatType = typeof(float);
        private static readonly Type doubleType = typeof(double);
        private static readonly Type stringType = typeof(string);
        private static readonly Type decimalType = typeof(decimal);
        private static readonly Type iSerializableType = typeof(ISerializable);
        private static readonly Type iListType = typeof(IList);
        private static readonly Type iDictionaryType = typeof(IDictionary);
        private static readonly Type dateTimeType = typeof(DateTime);
        private static readonly Type typeType = typeof(Type);
        private static readonly Type objectType = typeof(object);
    }
}