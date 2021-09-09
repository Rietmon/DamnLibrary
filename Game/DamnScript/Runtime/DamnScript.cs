#if UNITY_5_3_OR_NEWER  && ENABLE_DAMN_SCRIPT
using System.Collections.Generic;
#if ENABLE_SERIALIZATION
using Rietmon.Serialization;
#endif
using UnityEngine;

namespace Rietmon.DS
{
    public class DamnScript
    {
        public GameObject Owner { get; }

        /// <summary>
        /// Use only for serialization!!!
        /// </summary>
        public List<string> ExecutingDamnScriptRegions { get; set; }
    
        private readonly Dictionary<string, DamnScriptRegion> damnScriptRegions = new Dictionary<string, DamnScriptRegion>();

        public DamnScript(string scriptContent, GameObject owner)
        {
            Owner = owner;

            var regions = DamnScriptParser.ParseRegions(scriptContent);
            foreach (var region in regions)
                damnScriptRegions.Add(region.Key, new DamnScriptRegion(region.Value, () => OnRegionEnd(region.Key), this));
        }

        public async void BeginAsync(string regionName = "Start")
        {
            if (ExecutingDamnScriptRegions != null)
            {
                foreach (var region in ExecutingDamnScriptRegions)
                {
                    GetRegion(region).BeginAsync();
                }
            }
            else
            {
                ExecutingDamnScriptRegions = new List<string>();

                GetRegion(regionName).BeginAsync();
                OnRegionStart(regionName);
            }
        }

        public DamnScriptRegion GetRegion(string name)
        {
            if (!damnScriptRegions.TryGetValue(name, out var region))
            {
                Debug.LogError(
                    $"[{nameof(DamnScript)}] ({nameof(BeginAsync)}) Unable to find region with the name {name}");
                return null;
            }

            return region;
        }
        
#if ENABLE_SERIALIZATION
        public void Serialize(SerializationStream stream)
        {
            stream.Write(ExecutingDamnScriptRegions);
            
            foreach (var executingRegion in ExecutingDamnScriptRegions)
                damnScriptRegions[executingRegion].Serialize(stream);
        }

        public void Deserialize(SerializationStream stream)
        {
            ExecutingDamnScriptRegions = stream.Read<List<string>>();

            foreach (var executingRegion in ExecutingDamnScriptRegions)
                damnScriptRegions[executingRegion].Deserialize(stream);
        }
#endif

        private void OnRegionStart(string regionName) => ExecutingDamnScriptRegions.Add(regionName);

        private void OnRegionEnd(string regionName) => ExecutingDamnScriptRegions.Remove(regionName);
    }   
}
#endif