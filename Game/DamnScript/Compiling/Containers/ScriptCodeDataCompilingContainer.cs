#if ENABLE_SERIALIZATION && ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript.Data;
using DamnLibrary.Serialization;

namespace DamnLibrary.DamnScript.Compiling
{
    public class ScriptCodeDataCompilingContainer : ISerializable
    {
        private string[] codes;

        public ScriptCodeData ToData() => 
            new ScriptCodeData(codes);

        void ISerializable.Serialize(SerializationStream stream)
        {
            stream.Write(codes);
        }

        void ISerializable.Deserialize(SerializationStream stream)
        {
            codes = stream.Read<string[]>();
        }
        
        public static ScriptCodeDataCompilingContainer FromData(ScriptCodeData data)
        {
            return new ScriptCodeDataCompilingContainer
            {
                codes = data.codes
            };
        }
    }
}
#endif