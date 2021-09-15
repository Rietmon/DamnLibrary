#if ENABLE_DAMN_SCRIPT
using System.Collections.Generic;
using Rietmon.DamnScript.Data;
using Rietmon.DamnScript.Parsers;
using Rietmon.Debugging;
#if ENABLE_SERIALIZATION
using Rietmon.Serialization;
#endif

namespace Rietmon.DamnScript.Executing
{
    public class Script
    {
        public IScriptExecutor Parent { get; }

        public List<ScriptRegion> ExecutingRegions { get; } = new List<ScriptRegion>();

        public bool IsExecuting => ExecutingRegions.Count > 0;
        
        private readonly ScriptData scriptData;

        public Script(ScriptData scriptData, IScriptExecutor parent)
        {
            Parent = parent;
            this.scriptData = scriptData;
        }

        public void Begin(string regionName = "Main")
        {
            var region = CreateRegion(regionName);

            if (region == null)
            {
                UniversalDebugger.LogError(
                    $"{nameof(Script)} ({nameof(Begin)}) Unable to find region with the name {regionName}.");
                return;
            }
            
            region.Begin();
        }

        private ScriptRegion CreateRegion(string regionName)
        {
            var regionData = scriptData.GetRegionData(regionName);
            
            return regionData == null ? null : new ScriptRegion(this, regionData);
        }

        internal void OnRegionStart(ScriptRegion region) => 
            ExecutingRegions.Add(region);

        internal void OnRegionEnd(ScriptRegion region) => 
            ExecutingRegions.Remove(region);
    }   
}
#endif