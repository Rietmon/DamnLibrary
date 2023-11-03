#if ENABLE_DAMN_SCRIPT
using System.Collections.Generic;
using DamnLibrary.DamnScript.Data;
using DamnLibrary.Debugging;
#if ENABLE_SERIALIZATION
using DamnLibrary.Serialization;
#endif

namespace DamnLibrary.DamnScript.Executing
{
#if ENABLE_SERIALIZATION
    [DontCreateInstanceAtDeserialization]
#endif
    public class Script 
#if ENABLE_SERIALIZATION
        : ISerializable
#endif
    {
        /// <summary>
        /// Name of the script
        /// </summary>
        public string Name => ScriptData.Name;
        
        /// <summary>
        /// Script executor that is executing this script
        /// </summary>
        public IScriptExecutor Parent { get; }

        /// <summary>
        /// Is this script executing right now?
        /// </summary>
        public bool IsExecuting => ExecutingRegions.Count > 0;

        private List<ScriptRegion> ExecutingRegions { get; } = new();
        
        private ScriptData ScriptData { get; }

        public Script(ScriptData scriptData, IScriptExecutor parent)
        {
            Parent = parent;
            ScriptData = scriptData;
        }

        /// <summary>
        /// Start executing region with the given name
        /// </summary>
        /// <param name="regionName">Region name</param>
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
        
        /// <summary>
        /// Resume executing region with the given name
        /// </summary>
        /// <param name="regionName">Region name</param>
        public void ResumeRegion(string regionName)
        {
            if (!TryGetExecutingRegion(regionName, nameof(ResumeRegion), out var region))
                region.Resume();
        }

        /// <summary>
        /// Pause executing region with the given name
        /// </summary>
        /// <param name="regionName">Region name</param>
        public void PauseRegion(string regionName)
        {
            if (!TryGetExecutingRegion(regionName, nameof(PauseRegion), out var region))
                region.Pause();
        }

        /// <summary>
        /// Stop executing region with the given name
        /// </summary>
        /// <param name="regionName"></param>
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
                $"{nameof(Script)} ({methodName}) Unable to find executing region with the name {regionName}.");
            return false;
        }

        private ScriptRegion CreateRegion(string regionName)
        {
            var regionData = ScriptData.GetRegionData(regionName);
            
            return regionData == null ? null : new ScriptRegion(this, regionData);
        }

        internal void OnRegionStart(ScriptRegion region) => 
            ExecutingRegions.Add(region);

        internal void OnRegionEnd(ScriptRegion region) => 
            ExecutingRegions.Remove(region);

#if ENABLE_SERIALIZATION
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
#endif
    }   
}
#endif