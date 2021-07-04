using System.Collections.Generic;
using System.Linq;
using Rietmon.Serialization;
using UnityEngine;

namespace Rietmon.DS
{
    public class DamnScript
    {
        public GameObject Owner { get; }
    
        private readonly Dictionary<string, DamnScriptRegion> damnScriptRegions = new Dictionary<string, DamnScriptRegion>();

        private List<string> executingDamnScriptRegions;

        public DamnScript(string scriptContent, GameObject owner)
        {
            Owner = owner;

            var regions = DamnScriptParser.ParseRegions(scriptContent);
            foreach (var region in regions)
                damnScriptRegions.Add(region.Key, new DamnScriptRegion(region.Value, () => OnRegionEnd(region.Key), this));
        }

        public async void BeginAsync(string regionName = "Start")
        {
            if (executingDamnScriptRegions != null)
            {
                foreach (var region in executingDamnScriptRegions)
                {
                    GetRegion(region).BeginAsync();
                }
            }
            else
            {
                executingDamnScriptRegions = new List<string>();

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
        
        public void Serialize(SerializationStream stream)
        {
            stream.Write(executingDamnScriptRegions);
            
            foreach (var executingRegion in executingDamnScriptRegions)
                damnScriptRegions[executingRegion].Serialize(stream);
        }

        public void Deserialize(SerializationStream stream)
        {
            executingDamnScriptRegions = stream.Read<List<string>>();

            foreach (var executingRegion in executingDamnScriptRegions)
                damnScriptRegions[executingRegion].Deserialize(stream);
        }

        private void OnRegionStart(string regionName) => executingDamnScriptRegions.Add(regionName);

        private void OnRegionEnd(string regionName) => executingDamnScriptRegions.Remove(regionName);
    }   
}