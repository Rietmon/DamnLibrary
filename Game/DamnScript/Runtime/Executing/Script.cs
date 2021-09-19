#if ENABLE_DAMN_SCRIPT
using System.Collections.Generic;
using Rietmon.DamnScript.Data;
using Rietmon.Debugging;
#if ENABLE_SERIALIZATION
using Rietmon.Serialization;
#endif

namespace Rietmon.DamnScript.Executing
{
    [DontCreateInstanceAtDeserialization]
    public class Script : ISerializable
    {
        public string Name => scriptData.name;
        public IScriptExecutor Parent { get; }

        private List<ScriptRegion> ExecutingRegions { get; } = new List<ScriptRegion>();

        public bool IsExecuting => ExecutingRegions.Count > 0;
        
        private readonly ScriptData scriptData;

        public Script(ScriptData scriptData, IScriptExecutor parent)
        {
            Parent = parent;
            this.scriptData = scriptData;
        }

        public void StartRegion(string regionName = "Main")
        {
            var region = CreateRegion(regionName);
            if (region == null)
            {
                UniversalDebugger.LogError(
                    $"{nameof(Script)} ({nameof(StartRegion)}) Unable to find region DATA with the name {regionName}.");
                return;
            }
            region.Start();
        }
        
        public void ResumeRegion(string regionName)
        {
            if (!TryGetExecutingRegion(regionName, nameof(ResumeRegion), out var region))
                region.Resume();
        }

        public void PauseRegion(string regionName)
        {
            if (!TryGetExecutingRegion(regionName, nameof(PauseRegion), out var region))
                region.Pause();
        }

        public void StopRegion(string regionName)
        {
            if (TryGetExecutingRegion(regionName, nameof(StopRegion), out var region))
                region.Stop();
        }

        private bool TryGetExecutingRegion(string regionName, string methodName, out ScriptRegion region)
        {
            region = ExecutingRegions.Find((scriptRegion) => scriptRegion.Name == regionName);
            if (region != null) 
                return true;
            
            UniversalDebugger.LogError(
                $"{nameof(Script)} ({methodName}) Unable to find EXECUTING region with the name {regionName}.");
            return false;
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

        void ISerializable.Serialize(SerializationStream stream)
        {
            stream.Write((ushort)ExecutingRegions.Count);
            foreach (var executingRegion in ExecutingRegions)
            {
                stream.Write(executingRegion.Name);
                ((ISerializable)executingRegion).Serialize(stream);
            }
        }

        void ISerializable.Deserialize(SerializationStream stream)
        {
            var executingRegionsCount = stream.Read<ushort>();
            for (var i = 0; i < executingRegionsCount; i++)
            {
                var regionName = stream.Read<string>();
                var region = CreateRegion(regionName);
                ((ISerializable)region).Deserialize(stream);
            }
        }
    }   
}
#endif