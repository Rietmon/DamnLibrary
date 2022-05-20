#if ENABLE_SERIALIZATION && ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript.Data;
using DamnLibrary.Extensions;
using DamnLibrary.Serialization;

namespace DamnLibrary.DamnScript.Compiling
{
    internal class ScriptDataCompilingContainer : ISerializable
    {
        private string name;

        private ScriptRegionDataCompilingContainer[] regionsDataContainer;

        public ScriptData ToData() =>
            new ScriptData(name, regionsDataContainer.SmartCast((regionDataContainer) => regionDataContainer.ToData()));

        void ISerializable.Serialize(SerializationStream stream)
        {
            stream.Write(name);
            stream.Write(regionsDataContainer);
        }

        void ISerializable.Deserialize(SerializationStream stream)
        {
            name = stream.Read<string>();
            regionsDataContainer = stream.Read<ScriptRegionDataCompilingContainer[]>();
        }
        
        public static ScriptDataCompilingContainer FromData(ScriptData data)
        {
            return new ScriptDataCompilingContainer
            {
                name = data.name,
                regionsDataContainer = data.regionsData.SmartCast(ScriptRegionDataCompilingContainer.FromData)
            };
        }
    }
}
#endif