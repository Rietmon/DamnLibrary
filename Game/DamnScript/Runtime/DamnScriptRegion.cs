#if UNITY_2020 && ENABLE_DAMN_SCRIPT
using System;
using System.Collections.Generic;
#if ENABLE_UNI_TASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
#endif
using Rietmon.Serialization;

namespace Rietmon.DS
{
    public class DamnScriptRegion : ISerializable
    {
        public DamnScript Parent { get; }

        public bool IsOver => damnScriptCodes.Count <= CurrentMethod;

        public DamnScriptCode CurrentCode => IsOver ? null : damnScriptCodes[CurrentMethod];
    
        public int CurrentMethod { get; private set; }

        private readonly List<DamnScriptCode> damnScriptCodes = new List<DamnScriptCode>();

        private readonly Action onRegionEndCallback;
    
        private bool stopExecuting;

        public DamnScriptRegion(string code, Action onRegionEndCallback, DamnScript parent)
        {
            Parent = parent;
            this.onRegionEndCallback = onRegionEndCallback;

            var codes = DamnScriptParser.ParseCode(code);
            foreach (var codesArray in codes)
                damnScriptCodes.Add(new DamnScriptCode(codesArray, this));
        }

        public async void BeginAsync()
        {
            while (true)
            {
                if (IsOver || stopExecuting)
                {
#if ENABLE_UNI_TASK
                    await UniTask.Yield();
#else
                    await Task.Yield();
#endif
                    
                    stopExecuting = false;
                    onRegionEndCallback?.Invoke();
                    return;
                }

                if (await CurrentCode.ExecuteAsync()) 
                    CurrentMethod++;

#if ENABLE_UNI_TASK
                    await UniTask.Yield();
#else
                await Task.Yield();
#endif
            }
        }

        public void Stop()
        {
            stopExecuting = true;
        }

        public void Serialize(SerializationStream stream)
        {
            stream.Write(CurrentMethod);
        }

        public void Deserialize(SerializationStream stream)
        {
            CurrentMethod = stream.Read<int>();
        }
    }   
}
#endif