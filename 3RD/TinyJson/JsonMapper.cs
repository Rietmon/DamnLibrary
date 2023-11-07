using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DamnLibrary.Debugs;

namespace DamnLibrary.TinyJson
{
    using Encoder = Action<object, JsonBuilder>;
    using Decoder = Func<Type, object, object>;

    public static class JsonMapper
    {
        private static Encoder genericEncoder;
        private static Decoder genericDecoder;
        private static readonly IDictionary<Type, Encoder> encoders = new Dictionary<Type, Encoder>();
        private static readonly IDictionary<Type, Decoder> decoders = new Dictionary<Type, Decoder>();

        static JsonMapper()
        {
            // Register default encoder
            RegisterEncoder(typeof(object), DefaultEncoder.GenericEncoder());
            RegisterEncoder(typeof(IDictionary), DefaultEncoder.DictionaryEncoder());
            RegisterEncoder(typeof(IEnumerable), DefaultEncoder.EnumerableEncoder());
            RegisterEncoder(typeof(DateTime), DefaultEncoder.ZuluDateEncoder());

            // Register default decoder
            RegisterDecoder(typeof(object), DefaultDecoder.GenericDecoder());
            RegisterDecoder(typeof(IDictionary), DefaultDecoder.DictionaryDecoder());
            RegisterDecoder(typeof(Array), DefaultDecoder.ArrayDecoder());
            RegisterDecoder(typeof(IList), DefaultDecoder.ListDecoder());
            RegisterDecoder(typeof(ICollection), DefaultDecoder.CollectionDecoder());
            RegisterDecoder(typeof(IEnumerable), DefaultDecoder.EnumerableDecoder());
        }

        public static void RegisterDecoder(Type type, Decoder decoder)
        {
            if (type == typeof(object))
            {
                genericDecoder = decoder;
            }
            else
            {
                decoders.Add(type, decoder);
            }
        }

        public static void RegisterEncoder(Type type, Encoder encoder)
        {
            if (type == typeof(object))
            {
                genericEncoder = encoder;
            }
            else
            {
                encoders.Add(type, encoder);
            }
        }

        public static Decoder GetDecoder(Type type)
        {
            if (decoders.ContainsKey(type))
            {
                return decoders[type];
            }

            foreach (var entry in decoders)
            {
                var baseType = entry.Key;
                if (baseType.IsAssignableFrom(type))
                {
                    return entry.Value;
                }

                if (baseType.HasGenericInterface(type))
                {
                    return entry.Value;
                }
            }

            return genericDecoder;
        }

        public static Encoder GetEncoder(Type type)
        {
            if (encoders.ContainsKey(type))
            {
                return encoders[type];
            }

            foreach (var entry in encoders)
            {
                var baseType = entry.Key;
                if (baseType.IsAssignableFrom(type))
                {
                    return entry.Value;
                }

                if (baseType.HasGenericInterface(type))
                {
                    return entry.Value;
                }
            }

            return genericEncoder;
        }

        public static T DecodeJsonObject<T>(object jsonObj)
        {
            var decoder = GetDecoder(typeof(T));
            return (T)decoder(typeof(T), jsonObj);
        }

        public static object DecodeJsonObject(object jsonObj, Type type)
        {
            var decoder = GetDecoder(type);
            return decoder(type, jsonObj);
        }

        public static void EncodeValue(object value, JsonBuilder builder)
        {
            if (JsonBuilder.IsSupported(value))
            {
                builder.AppendValue(value);
            }
            else
            {
                var encoder = GetEncoder(value.GetType());
                if (encoder != null)
                {
                    encoder(value, builder);
                }
                else
                {
                    UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(EncodeValue)}) Encoder for " + value.GetType() + " not found");
                }
            }
        }

        public static void EncodeNameValue(string name, object value, JsonBuilder builder)
        {
            builder.AppendName(name);
            EncodeValue(value, builder);
        }

        public static object DecodeValue(object value, Type targetType)
        {
            if (value == null) return null;

            if (JsonBuilder.IsSupported(value))
            {
                value = ConvertValue(value, targetType);
            }

            // use a registered decoder
            if (value != null && !targetType.IsInstanceOfType(value))
            {
                var decoder = GetDecoder(targetType);
                value = decoder(targetType, value);
            }

            if (value != null && targetType.IsInstanceOfType(value))
            {
                return value;
            }

            UniversalDebugger.LogError($"[{nameof(JsonMapper)}] ({nameof(DecodeValue)}) Couldn't decode: " + targetType);
            return null;
        }

        public static bool DecodeValue(object target, string name, object value)
        {
            var type = target.GetType();
            while (type != null)
            {
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                                                     BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length == 0)
                    {
                        var fieldName = field.UnwrappedFieldName(type);

                        if (name.Equals(fieldName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (value != null)
                            {
                                var targetType = field.FieldType;
                                var decodedValue = DecodeValue(value, targetType);

                                if (decodedValue != null && targetType.IsInstanceOfType(decodedValue))
                                {
                                    field.SetValue(target, decodedValue);
                                    return true;
                                }

                                return false;
                            }

                            field.SetValue(target, null);
                            return true;
                        }
                    }
                }

                type = type.BaseType;
            }

            return false;
        }
        
        private static object ConvertValue(object value, Type type)
        {
            if (value != null)
            {
                var safeType = Nullable.GetUnderlyingType(type) ?? type;
                if (!type.IsEnum)
                {
                    return Convert.ChangeType(value, safeType);
                }

                if (value is string)
                {
                    return Enum.Parse(type, (string)value);
                }

                return Enum.ToObject(type, value);
            }

            return value;
        }
    }
}