#if ENABLE_SERIALIZATION && ENABLE_DAMN_SCRIPT
using System.Runtime.Serialization;
using DamnLibrary.Games.DamnScript.Runtime.Data;

namespace DamnLibrary.Games.DamnScript.Compiling.Containers
{
    public class ScriptCodeDataCompilingContainer : ISerializable
    {
        private string[] codes;

        /// <summary>
        /// Create a ScriptCodeData from this ScriptCodeDataCompilingContainer
        /// </summary>
        /// <returns>ScriptCodeData</returns>
        public ScriptCodeData ToData() => new(codes);

        void ISerializable.Serialize(SerializationStream stream)
        {
            stream.Write(codes);
        }

        void ISerializable.Deserialize(SerializationStream stream)
        {
            codes = stream.Read<string[]>();
        }
        
        /// <summary>
        /// Create a new ScriptCodeDataCompilingContainer from a ScriptCodeData
        /// </summary>
        /// <param name="data">ScriptCodeData</param>
        /// <returns>ScriptCodeDataCompilingContainer</returns>
        public static ScriptCodeDataCompilingContainer FromData(ScriptCodeData data)
        {
            return new ScriptCodeDataCompilingContainer
            {
                codes = data.Codes
            };
        }
    }
}
#endif