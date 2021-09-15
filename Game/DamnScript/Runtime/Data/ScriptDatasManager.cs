using System.Collections.Generic;
using Rietmon.DamnScript.Parsers;

namespace Rietmon.DamnScript.Data
{
    public static class ScriptDatasManager
    {
        private static readonly Dictionary<string, ScriptData> scriptsData = new Dictionary<string, ScriptData>();

        public static ScriptData Get(string name) =>
            scriptsData.TryGetValue(name, out var scriptData) ? scriptData : null;

        public static ScriptData Create(string name, string code)
        {
            var regions = ScriptParser.ParseRegions(code);
            var regionsData = new ScriptRegionData[regions.Length];
            for (var i = 0; i < regions.Length; i++)
            {
                var currentRegion = regions[i];
                var regionCodes = ScriptParser.ParseCode(currentRegion.Second);

                var codesData = new ScriptCodeData[regionCodes.GetLength(0)];
                for (var j = 0; j < codesData.Length; j++)
                {
                    codesData[j] = new ScriptCodeData(regionCodes[i]);
                }

                regionsData[i] = new ScriptRegionData(currentRegion.First, codesData);
            }

            var scriptData = new ScriptData(name, regionsData);
            
            scriptsData.Add(name, scriptData);
            return scriptData;
        }
    }
}