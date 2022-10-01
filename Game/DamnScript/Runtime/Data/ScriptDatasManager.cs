#if ENABLE_DAMN_SCRIPT
using System.Collections.Generic;
using DamnLibrary.DamnScript.Parsers;
using DamnLibrary.Debugging;

namespace DamnLibrary.DamnScript.Data
{
    public static class ScriptDatasManager
    {
        private static readonly Dictionary<string, ScriptData> scriptsData = new();

        public static ScriptData Get(string name) =>
            scriptsData.TryGetValue(name, out var scriptData) ? scriptData : null;

        public static ScriptData Create(string name, string code)
        {
            var debugStopwatch = new DebugStopwatch();
            debugStopwatch.Start();
            
            var regions = ScriptParser.ParseRegions(code);
            var regionsData = new ScriptRegionData[regions.Length];
            for (var i = 0; i < regions.Length; i++)
            {
                var currentRegion = regions[i];
                var regionCodes = ScriptParser.ParseCode(currentRegion.Second);

                var codesData = new ScriptCodeData[regionCodes.GetLength(0)];
                for (var j = 0; j < codesData.Length; j++)
                {
                    codesData[j] = new ScriptCodeData(regionCodes[j]);
                }

                regionsData[i] = new ScriptRegionData(currentRegion.First, codesData);
            }

            var scriptData = new ScriptData(name, regionsData);
            
            scriptsData.Add(name, scriptData);
            
            debugStopwatch.Stop("Parsed ScriptData in {0} ms.");
            
            return scriptData;
        }
    }
}
#endif