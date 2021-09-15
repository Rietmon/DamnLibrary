using System.Collections.Generic;

namespace Rietmon.DamnScript.Data
{
    public class ScriptRegionData
    {
        public int CodesCount => codesData.Length;
        
        public readonly string name;

        private readonly ScriptCodeData[] codesData;

        public ScriptRegionData(string name, ScriptCodeData[] codesData)
        {
            this.name = name;
            this.codesData = codesData;
        }

        public ScriptCodeData GetCodeData(int index) => 
            index >= codesData.Length ? null : codesData[index];
    }
}