#if ENABLE_SERIALIZATION && ENABLE_DAMN_SCRIPT
using Rietmon.DamnScript.Data;
using Rietmon.Extensions;
using Rietmon.Serialization;

namespace Rietmon.DamnScript.Compiling
{
    public class ScriptRegionDataCompilingContainer : ISerializable
    {
        private string name;

        private ScriptCodeDataCompilingContainer[] codesDataContainer;

        public ScriptRegionData ToData() =>
            new ScriptRegionData(name, codesDataContainer.SmartCast((codeDataContainer) => codeDataContainer.ToData()));

        void ISerializable.Serialize(SerializationStream stream)
        {
            stream.Write(name);
            stream.Write(codesDataContainer);
        }

        void ISerializable.Deserialize(SerializationStream stream)
        {
            name = stream.Read<string>();
            codesDataContainer = stream.Read<ScriptCodeDataCompilingContainer[]>();
        }
        
        public static ScriptRegionDataCompilingContainer FromData(ScriptRegionData data)
        {
            return new ScriptRegionDataCompilingContainer
            {
                name = data.name,
                codesDataContainer = data.codesData.SmartCast(ScriptCodeDataCompilingContainer.FromData)
            };
        }
    }
}
#endif