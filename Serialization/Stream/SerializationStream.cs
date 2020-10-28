using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace Rietmon.Serialization
{
    public class SerializationStream
    {
        public bool IsReading { get; }
    
        public bool IsWriting { get; }
        
        public int Version { get; set; }

        private readonly MemoryStream stream;

        public SerializationStream()
        {
            IsReading = true;
            IsWriting = false;
            
            stream = new MemoryStream();
        }

        public SerializationStream(byte[] data)
        {
            IsReading = false;
            IsWriting = true;
        
            stream = new MemoryStream(data);
        }
        
        // WRITING

        public void Write<T>(T obj) => WriteWithSize(ToBytes(obj));

        public byte[] ToBytes<T>(T obj)
        {
            switch (obj)
            {
                case bool result: { return BitConverter.GetBytes(result); }
                case byte result: { return BitConverter.GetBytes(result); }
                case int result: { return BitConverter.GetBytes(result); }
                case float result: { return BitConverter.GetBytes(result); }
                case double result: { return BitConverter.GetBytes(result); }
                case string result: { return Encoding.UTF8.GetBytes(result); }
                case ISerializable result: { return GetBytes(result); }
                case Array result: { return GetBytes(result); }
                case IList result: { return GetBytes(result); }
                case Vector2 result: { return GetBytes(result); }
                case Vector3 result: { return GetBytes(result); }
                case Quaternion result: { return GetBytes(result); }
            }
        
            Debug.LogError($"Cannot convert to bytes unsupported type: {typeof(T)}!");
            return null;
        }

        #region GetBytes

        private byte[] GetBytes(ISerializable serializableComponent)
        {
            var subStream = new SerializationStream();
            subStream.Version = Version;
        
            serializableComponent.Serialize(subStream);

            return subStream.ToArray();
        }

        private byte[] GetBytes(Array array)
        {
            var subStream = new SerializationStream();
            subStream.Version = Version;

            var arrayLength = array.Length;
        
            subStream.Write(arrayLength);
        
            for (var i = 0; i < arrayLength; i++)
                subStream.Write(array.GetValue(i));

            return subStream.ToArray();
        }

        private byte[] GetBytes(IList list)
        {
            var subStream = new SerializationStream();
            subStream.Version = Version;

            var arrayLength = list.Count;
        
            subStream.Write(arrayLength);
        
            for (var i = 0; i < arrayLength; i++)
                subStream.Write(list[i]);

            return subStream.ToArray();
        }
        
        private byte[] GetBytes(Vector2 vector2)
        {
            var subStream = new SerializationStream();
            subStream.Version = Version;
            
            subStream.Write(vector2.x);
            subStream.Write(vector2.y);

            return subStream.ToArray();
        }

        private byte[] GetBytes(Vector3 vector3)
        {
            var subStream = new SerializationStream();
            subStream.Version = Version;
            
            subStream.Write(vector3.x);
            subStream.Write(vector3.y);
            subStream.Write(vector3.z);

            return subStream.ToArray();
        }

        private byte[] GetBytes(Quaternion quaternion)
        {
            var subStream = new SerializationStream();
            subStream.Version = Version;
            
            subStream.Write(quaternion.x);
            subStream.Write(quaternion.y);
            subStream.Write(quaternion.z);
            subStream.Write(quaternion.w);

            return subStream.ToArray();
        }

        #endregion

        private void WriteWithSize(byte[] bytes)
        {
            WriteSize(bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        private void WriteSize(int size)
        {
            var length = BitConverter.GetBytes(size);
        
            stream.Write(length, 0, 4);
        }
        
        // READING

        public T Read<T>() => (T)ToObject(typeof(T));
    
        private object Read(Type type) => ToObject(type);

        private object ToObject(Type type)
        {
            if (type == typeof(bool)) return BitConverter.ToBoolean(ReadBySize(), 0);
            if (type == typeof(byte)) return ReadBySize()[0];
            if (type == typeof(int)) return BitConverter.ToInt32(ReadBySize(), 0);
            if (type == typeof(float)) return BitConverter.ToSingle(ReadBySize(), 0);
            if (type == typeof(double)) return BitConverter.ToDouble(ReadBySize(), 0);
            if (type == typeof(string)) return Encoding.UTF8.GetString(ReadBySize());
            if (typeof(ISerializable).IsAssignableFrom(type)) return GetSerializableObject(type);
            if (type.IsArray) return GetArrayObject(type);
            if (typeof(IList).IsAssignableFrom(type)) return GetListObject(type);
            if (typeof(Vector2).IsAssignableFrom(type)) return GetVector2Object(type);
            if (typeof(Vector3).IsAssignableFrom(type)) return GetVector3Object(type);
            if (typeof(Quaternion).IsAssignableFrom(type)) return GetQuaternionObject(type);
        
            Debug.LogError($"Cannot convert to object unsupported type: {type}!");
        
            return null;
        }

        #region GetObject's

        private object GetSerializableObject(Type type)
        {
            var result = Activator.CreateInstance(type);
        
            var subStream = new SerializationStream(ReadBySize());
            subStream.Version = Version;

            ((ISerializable)result).Deserialize(subStream);

            return result;
        }

        private object GetArrayObject(Type type)
        {
            var elementType = type.GetElementType();
        
            var subStream = new SerializationStream(ReadBySize());
            subStream.Version = Version;
        
            var array = Array.CreateInstance(elementType, subStream.Read<int>());
        
            for (var i = 0; i < array.Length; i++)
                array.SetValue(subStream.Read(elementType), i);

            return array;
        }

        private object GetListObject(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
        
            var subStream = new SerializationStream(ReadBySize());
            subStream.Version = Version;

            var array = (IList)Activator.CreateInstance(type);

            var arrayLength = subStream.Read<int>();
        
            for (var i = 0; i < arrayLength; i++)
                array.Add(subStream.Read(elementType));

            return array;
        }
        
        private object GetVector2Object(Type type)
        {
            var result = (Vector2)Activator.CreateInstance(type);
        
            var subStream = new SerializationStream(ReadBySize());
            subStream.Version = Version;

            result.x = subStream.Read<float>();
            result.y = subStream.Read<float>();

            return result;
        }
        
        private object GetVector3Object(Type type)
        {
            var result = (Vector3)Activator.CreateInstance(type);
        
            var subStream = new SerializationStream(ReadBySize());
            subStream.Version = Version;

            result.x = subStream.Read<float>();
            result.y = subStream.Read<float>();
            result.z = subStream.Read<float>();

            return result;
        }
        
        private object GetQuaternionObject(Type type)
        {
            var result = (Quaternion)Activator.CreateInstance(type);
        
            var subStream = new SerializationStream(ReadBySize());
            subStream.Version = Version;

            result.x = subStream.Read<float>();
            result.y = subStream.Read<float>();
            result.z = subStream.Read<float>();
            result.w = subStream.Read<float>();

            return result;
        }

        #endregion
        
        private byte[] ReadBySize()
        {
            var size = ReadSize();

            var buffer = new byte[size];

            stream.Read(buffer, 0, size);

            return buffer;
        }

        private int ReadSize()
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);

            return BitConverter.ToInt32(buffer, 0);
        }

        public byte[] ToArray() => stream.ToArray();

        ~SerializationStream()
        {
            stream.Dispose();   
        }
    }
}
