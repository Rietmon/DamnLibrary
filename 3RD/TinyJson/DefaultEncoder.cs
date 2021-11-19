using System;
using System.Collections;
using System.Reflection;

namespace Tiny
{
    using Encoder = Action<object, JsonBuilder>;

    public static class DefaultEncoder
    {
        public static Encoder GenericEncoder()
        {
            return (obj, builder) =>
            {
                builder.AppendBeginObject();
                var type = obj.GetType();
                var first = true;
                while (type != null)
                {
                    foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                                                         BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    {
                        if (field.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length == 0)
                        {
                            if (first) first = false;
                            else builder.AppendSeperator();

                            var fieldName = field.UnwrappedFieldName(type);
                            JsonMapper.EncodeNameValue(fieldName, field.GetValue(obj), builder);
                        }
                    }

                    type = type.BaseType;
                }

                builder.AppendEndObject();
            };
        }

        public static Encoder DictionaryEncoder()
        {
            return (obj, builder) =>
            {
                builder.AppendBeginObject();
                var first = true;
                var dict = (IDictionary)obj;
                foreach (var key in dict.Keys)
                {
                    if (first) first = false;
                    else builder.AppendSeperator();
                    JsonMapper.EncodeNameValue(key.ToString(), dict[key], builder);
                }

                builder.AppendEndObject();
            };
        }

        public static Encoder EnumerableEncoder()
        {
            return (obj, builder) =>
            {
                builder.AppendBeginArray();
                var first = true;
                foreach (var item in (IEnumerable)obj)
                {
                    if (first) first = false;
                    else builder.AppendSeperator();
                    JsonMapper.EncodeValue(item, builder);
                }

                builder.AppendEndArray();
            };
        }

        public static Encoder ZuluDateEncoder()
        {
            return (obj, builder) =>
            {
                var date = (DateTime)obj;
                var zulu = date.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                builder.AppendString(zulu);
            };
        }
    }
}