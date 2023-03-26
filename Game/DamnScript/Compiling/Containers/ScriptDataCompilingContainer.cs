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

        /// <summary>
        /// Create a ScriptData from this ScriptDataCompilingContainer
        /// </summary>
        /// <returns>ScriptData</returns>
        public ScriptData ToData() =>
            new(name, regionsDataContainer.FuncCast((regionDataContainer) => regionDataContainer.ToData()));

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

        /// <summary>
        /// Create a new ScriptDataCompilingContainer from a ScriptData
        /// </summary>
        /// <param name="data">ScriptData</param>
        /// <returns>ScriptDataCompilingContainer</returns>
        public static ScriptDataCompilingContainer FromData(ScriptData data)
        {
            return new ScriptDataCompilingContainer
            {
                name = data.Name,
                regionsDataContainer = data.RegionsData.FuncCast(ScriptRegionDataCompilingContainer.FromData)
            };
        }
    }
}
#endif