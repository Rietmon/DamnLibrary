using System;

namespace DamnLibrary.TinyJson
{
    public static class Json
    {
        public static T Decode<T>(this string json)
        {
            if (string.IsNullOrEmpty(json)) 
                return default;
            var jsonObj = JsonParser.ParseValue(json);
            
            return jsonObj == null ? default : JsonMapper.DecodeJsonObject<T>(jsonObj);
        }

        public static object Decode(this string json, Type type)
        {
            if (string.IsNullOrEmpty(json)) 
                return null;
            
            var jsonObj = JsonParser.ParseValue(json);
            return jsonObj == null ? null : JsonMapper.DecodeJsonObject(jsonObj, type);
        }

        public static string Encode(this object value, bool pretty = false)
        {
            var builder = new JsonBuilder(pretty);
            JsonMapper.EncodeValue(value, builder);
            return builder.ToString();
        }
    }
}