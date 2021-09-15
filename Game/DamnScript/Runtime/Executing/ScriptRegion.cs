#if ENABLE_DAMN_SCRIPT
using System;
using System.Collections.Generic;
using Rietmon.DamnScript.Parsers;
#if ENABLE_UNI_TASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
using Rietmon.DamnScript.Data;
#endif
#if ENABLE_SERIALIZATION
using Rietmon.Serialization;
#endif

namespace Rietmon.DamnScript.Executing
{
    public class ScriptRegion
    {
        public Script Parent { get; }

        public List<ScriptCode> ExecutingCodes { get; } = new List<ScriptCode>();

        public bool IsOver => currentCodeIndex >= regionData.CodesCount;

        public bool IsPaused { get; private set; }
        
        public bool IsStopped { get; private set; }

        private readonly ScriptRegionData regionData;

        private int currentCodeIndex;

        public ScriptRegion(Script parent, ScriptRegionData regionData)
        {
            Parent = parent;
            this.regionData = regionData;
        }

        public void Begin()
        {
            IsPaused = false;
            IsStopped = false;
            currentCodeIndex = 0;
            Parent.OnRegionStart(this);
            Handler();
        }

        public void Pause() => IsPaused = true;

        public void Stop() => IsStopped = true;

        private async void Handler()
        {
            while (true)
            {
                if (IsOver || IsStopped)
                {
                    await Task.Yield();
                    
                    IsStopped = false;
                    Parent.OnRegionEnd(this);
                    return;
                }

                var code = CreateCode(currentCodeIndex);
                if (await code.ExecuteAsync()) 
                    currentCodeIndex++;

                await Task.Yield();
            }
        }

        private ScriptCode CreateCode(int index)
        {
            var codeData = regionData.GetCodeData(index);
            
            return codeData == null ? null : new ScriptCode(this, codeData);
        }
    }   
}
#endif