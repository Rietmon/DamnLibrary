#if ENABLE_DAMN_SCRIPT
namespace DamnLibrary.Games.DamnScript.Runtime.Data
{
    public class ScriptRegionData
    {
        public int CodesCount => CodesData.Length;
        
        public string Name { get; }

        internal ScriptCodeData[] CodesData { get; }

        public ScriptRegionData(string name, ScriptCodeData[] codesData)
        {
            Name = name;
            CodesData = codesData;
        }

        /// <summary>
        /// Return code data with the given index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>ScriptCodeData</returns>
        public ScriptCodeData GetCodeData(int index) => 
            index >= CodesData.Length ? null : CodesData[index];
    }
}
#endif