#if ENABLE_DAMN_SCRIPT
using System.Collections.Generic;
using System.Diagnostics;
using DamnLibrary.DamnScript.Parsers;
using DamnLibrary.Debugs;

namespace DamnLibrary.DamnScript.Runtime.Data
{
    internal static class ScriptDatasManager
    {
        private static readonly Dictionary<string, ScriptData> scriptsData = new();

        public static ScriptData Get(string name) =>
            scriptsData.TryGetValue(name, out var scriptData) ? scriptData : null;

        public static ScriptData Create(string name, string code)
        {
#if DEBUG
            var debugStopwatch = new Stopwatch();
            debugStopwatch.Start();
#endif
            
            var regions = ScriptParser.ParseRegions(code);
            var regionsData = new ScriptRegionData[regions.Length];
            for (var i = 0; i < regions.Length; i++)
            {
                var currentRegion = regions[i];
                var regionCodes = ScriptParser.ParseCode(currentRegion.Item2);

                var codesData = new ScriptCodeData[regionCodes.GetLength(0)];
                for (var j = 0; j < codesData.Length; j++)
                {
                    codesData[j] = new ScriptCodeData(regionCodes[j]);
                }

                regionsData[i] = new ScriptRegionData(currentRegion.Item1, codesData);
            }

            var scriptData = new ScriptData(name, regionsData);
            
            scriptsData.Add(name, scriptData);
            
#if DEBUG
            debugStopwatch.Stop();
            UniversalDebugger.Log(
                $"[{nameof(ScriptDatasManager)}] ({nameof(Create)}) Parsed ScriptData in {debugStopwatch.ElapsedMilliseconds} ms");
#endif
            
            return scriptData;
        }
    }
}
#endif