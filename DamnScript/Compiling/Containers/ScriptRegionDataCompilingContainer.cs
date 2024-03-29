﻿#if ENABLE_SERIALIZATION && ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript.Runtime.Data;
using DamnLibrary.Serializations;
using DamnLibrary.Serializations.Serializables;
using DamnLibrary.Utilities.Extensions;

namespace DamnLibrary.DamnScript.Compiling.Containers
{
    public class ScriptRegionDataCompilingContainer : ISerializable
    {
        private string name;

        private ScriptCodeDataCompilingContainer[] codesDataContainer;

        /// <summary>
        /// Create a ScriptRegionData from this ScriptRegionDataCompilingContainer
        /// </summary>
        /// <returns>ScriptRegionData</returns>
        public ScriptRegionData ToData() =>
            new(name, codesDataContainer.FuncCast((codeDataContainer) => codeDataContainer.ToData()));

        void ISerializable.Serialize(SerializationStream stream)
        {
            stream.Write(name);
            stream.Write(codesDataContainer);
        }

        void ISerializable.Deserialize(SerializationStream stream)
        {
            name = stream.ReadString();
            codesDataContainer = stream.ReadArray<ScriptCodeDataCompilingContainer>();
        }
        
        /// <summary>
        /// Create a new ScriptRegionDataCompilingContainer from a ScriptRegionData
        /// </summary>
        /// <param name="data">ScriptRegionData</param>
        /// <returns>ScriptRegionDataCompilingContainer</returns>
        public static ScriptRegionDataCompilingContainer FromData(ScriptRegionData data)
        {
            return new ScriptRegionDataCompilingContainer
            {
                name = data.Name,
                codesDataContainer = data.CodesData.FuncCast(ScriptCodeDataCompilingContainer.FromData)
            };
        }
    }
}
#endif