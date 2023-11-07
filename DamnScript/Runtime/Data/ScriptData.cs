#if ENABLE_DAMN_SCRIPT
using DamnLibrary.Utilities.Extensions;

namespace DamnLibrary.DamnScript.Runtime.Data
{
    public class ScriptData
    {
        public string Name { get; }

        internal ScriptRegionData[] RegionsData { get; }

        public ScriptData(string name, ScriptRegionData[] regionsData)
        {
            Name = name;
            RegionsData = regionsData;
        }

        /// <summary>
        /// Return ScriptRegionData with the given name
        /// </summary>
        /// <param name="regionName">Region name</param>
        /// <returns>ScriptRegionData</returns>
        public ScriptRegionData GetRegionData(string regionName) => 
            RegionsData.FindOrDefault((data) => data.Name == regionName);
    }
}
#endif