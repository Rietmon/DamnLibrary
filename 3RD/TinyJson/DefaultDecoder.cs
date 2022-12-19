using System;
using System.Collections;
using System.Collections.Generic;
using DamnLibrary.Debugging;

namespace Tiny
{
    using Decoder = Func<Type, object, object>;

    public static class DefaultDecoder
    {
        public static Decoder GenericDecoder()
        {
            return (type, jsonObj) =>
            {
                var instance = Activator.CreateInstance(type, true);
                if (jsonObj is IDictionary dictionary)
                {
                    foreach (DictionaryEntry item in dictionary)
                    {
                        var name = (string)item.Key;
                        if (!JsonMapper.DecodeValue(instance, name, item.Value))
                        {
                            UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(GenericDecoder)}) Couldn't decode field \"" + name + "\" of " + type);
                        }
                    }
                }
                else
                {
                    UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(GenericDecoder)}) Unsupported json type: " +
                                               (jsonObj != null ? jsonObj.GetType().ToString() : "null"));
                }

                return instance;
            };
        }

        public static Decoder DictionaryDecoder()
        {
            return (type, jsonObj) =>
            {
                // Dictionary
                if (jsonObj is IDictionary<string, object>)
                {
                    var jsonDict = (Dictionary<string, object>)jsonObj;
                    if (type.GetGenericArguments().Length == 2)
                    {
                        IDictionary instance = null;
                        var keyType = type.GetGenericArguments()[0];
                        var genericType = type.GetGenericArguments()[1];
                        var nullable = genericType.IsNullable();
                        if (type != typeof(IDictionary) && typeof(IDictionary).IsAssignableFrom(type))
                        {
                            instance = Activator.CreateInstance(type, true) as IDictionary;
                        }
                        else
                        {
                            var genericDictType = typeof(Dictionary<,>).MakeGenericType(keyType, genericType);
                            instance = Activator.CreateInstance(genericDictType) as IDictionary;
                        }

                        foreach (var item in jsonDict)
                        {
                            var value = JsonMapper.DecodeValue(item.Value, genericType);
                            object key = item.Key;
                            if (keyType == typeof(int)) key = int.Parse(item.Key);
                            if (value != null || nullable) instance.Add(key, value);
                        }

                        return instance;
                    }

                    UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(DictionaryDecoder)}) Unexpected type arguments");
                }

                // Dictionary (convert int to string key)
                if (jsonObj is IDictionary<int, object>)
                {
                    var jsonDict = new Dictionary<string, object>();
                    foreach (var keyValuePair in (Dictionary<int, object>)jsonObj)
                    {
                        jsonDict.Add(keyValuePair.Key.ToString(), keyValuePair.Value);
                    }

                    if (type.GetGenericArguments().Length == 2)
                    {
                        IDictionary instance = null;
                        var keyType = type.GetGenericArguments()[0];
                        var genericType = type.GetGenericArguments()[1];
                        var nullable = genericType.IsNullable();
                        if (type != typeof(IDictionary) && typeof(IDictionary).IsAssignableFrom(type))
                        {
                            instance = Activator.CreateInstance(type, true) as IDictionary;
                        }
                        else
                        {
                            var genericDictType = typeof(Dictionary<,>).MakeGenericType(keyType, genericType);
                            instance = Activator.CreateInstance(genericDictType) as IDictionary;
                        }

                        foreach (var item in jsonDict)
                        {
                            var value = JsonMapper.DecodeValue(item.Value, genericType);
                            if (value != null || nullable) instance.Add(Convert.ToInt32(item.Key), value);
                        }

                        return instance;
                    }

                    UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(DictionaryDecoder)}) Unexpected type arguments");
                }

                UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(DictionaryDecoder)}) Couldn't decode Dictionary: " + type);
                return null;
            };
        }

        public static Decoder ArrayDecoder()
        {
            return (type, jsonObj) =>
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    if (jsonObj is IList)
                    {
                        var jsonList = (IList)jsonObj;
                        if (type.IsArray)
                        {
                            var elementType = type.GetElementType();
                            var nullable = elementType.IsNullable();
                            var array = Array.CreateInstance(elementType, jsonList.Count);
                            for (var i = 0; i < jsonList.Count; i++)
                            {
                                var value = JsonMapper.DecodeValue(jsonList[i], elementType);
                                if (value != null || nullable) array.SetValue(value, i);
                            }

                            return array;
                        }
                    }
                }

                UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(ArrayDecoder)}) Couldn't decode Array: " + type);
                return null;
            };
        }

        public static Decoder ListDecoder()
        {
            return (type, jsonObj) =>
            {
                if (type.HasGenericInterface(typeof(IList<>)) && type.GetGenericArguments().Length == 1)
                {
                    var genericType = type.GetGenericArguments()[0];
                    if (jsonObj is IList)
                    {
                        var jsonList = (IList)jsonObj;
                        IList instance = null;
                        var nullable = genericType.IsNullable();
                        if (type != typeof(IList) && typeof(IList).IsAssignableFrom(type))
                        {
                            instance = Activator.CreateInstance(type, true) as IList;
                        }
                        else
                        {
                            var genericListType = typeof(List<>).MakeGenericType(genericType);
                            instance = Activator.CreateInstance(genericListType) as IList;
                        }

                        foreach (var item in jsonList)
                        {
                            var value = JsonMapper.DecodeValue(item, genericType);
                            if (value != null || nullable) instance.Add(value);
                        }

                        return instance;
                    }
                }

                UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(ListDecoder)}) Couldn't decode List: " + type);
                return null;
            };
        }

        public static Decoder CollectionDecoder()
        {
            return (type, jsonObj) =>
            {
                if (type.HasGenericInterface(typeof(ICollection<>)))
                {
                    var genericType = type.GetGenericArguments()[0];
                    if (jsonObj is IList jsonList)
                    {
                        var listType = type.IsInstanceOfGenericType(typeof(HashSet<>))
                            ? typeof(HashSet<>)
                            : typeof(List<>);
                        var constructedListType = listType.MakeGenericType(genericType);
                        var instance = Activator.CreateInstance(constructedListType, true);
                        var nullable = genericType.IsNullable();
                        var addMethodInfo = type.GetMethod("Add");
                        if (addMethodInfo != null)
                        {
                            foreach (var item in jsonList)
                            {
                                var value = JsonMapper.DecodeValue(item, genericType);
                                if (value != null || nullable) addMethodInfo.Invoke(instance, new object[] { value });
                            }

                            return instance;
                        }
                    }
                }

                UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(CollectionDecoder)}) Couldn't decode Collection: " + type);
                return null;
            };
        }

        public static Decoder EnumerableDecoder()
        {
            return (type, jsonObj) =>
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    // It could be an dictionary
                    if (jsonObj is IDictionary)
                    {
                        // Decode a dictionary
                        return DictionaryDecoder().Invoke(type, jsonObj);
                    }

                    // Or it could be also be a list
                    if (jsonObj is IList)
                    {
                        // Decode an array
                        if (type.IsArray)
                        {
                            return ArrayDecoder().Invoke(type, jsonObj);
                        }

                        // Decode a list
                        if (type.HasGenericInterface(typeof(IList<>)))
                        {
                            return ListDecoder().Invoke(type, jsonObj);
                        }

                        // Decode a collection
                        if (type.HasGenericInterface(typeof(ICollection<>)))
                        {
                            return CollectionDecoder().Invoke(type, jsonObj);
                        }
                    }
                }

                UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(EnumerableDecoder)}) Couldn't decode Enumerable: " + type);
                return null;
            };
        }
    }
}