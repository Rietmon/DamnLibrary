#if ENABLE_DAMN_SCRIPT
using System.Threading.Tasks;
using Rietmon.DamnScript.Data;
using Rietmon.Extensions;
#if ENABLE_SERIALIZATION
using Rietmon.Serialization;
#endif

namespace Rietmon.DamnScript.Executing
{
    [DamnScriptable
#if ENABLE_SERIALIZATION
     , DontCreateInstanceAtDeserialization
#endif
    ]
    public class ScriptRegion 
#if ENABLE_SERIALIZATION
        : ISerializable
#endif
    {
        public string Name => regionData.name;
        public Script Parent { get; }

        public bool IsOver => currentCodeIndex >= regionData.CodesCount;

        public bool IsPaused { get; private set; }
        
        public bool IsStopping { get; private set; }
        
        public bool IsExecuting { get; private set; }

        private readonly ScriptRegionData regionData;
        
        private int currentCodeIndex;

        public ScriptRegion(Script parent, ScriptRegionData regionData)
        {
            Parent = parent;
            this.regionData = regionData;
        }

        public void Start(bool resetValues = true)
        {
            if (resetValues)
            {
                IsPaused = false;
                IsStopping = false;
                currentCodeIndex = 0;
            }
            Parent.OnRegionStart(this);
            Handler();
        }

        public void Resume() => IsPaused = false;

        public void Pause() => IsPaused = true;

        public void Stop() => IsStopping = true;

        public void ForceNextCode() => currentCodeIndex++;

        private async void Handler()
        {
            IsExecuting = true;

            var stopTask = TaskUtilities.WaitUntil(() => IsStopping);
            while (!IsOver)
            {
                await Task.WhenAny(
                    TaskUtilities.WaitUntil(() => !IsPaused), 
                    stopTask);
                
                var code = CreateCode(currentCodeIndex);
                var codeTask = code.ExecuteAsync();
                await Task.WhenAny(codeTask, stopTask);

                if (stopTask.IsCompleted)
                {
                    IsStopping = false;
                    Parent.OnRegionEnd(this);
                    break;
                }

                if (codeTask.IsCompleted && codeTask.Result)
                    currentCodeIndex++;

                await Task.Yield();
            }

            IsExecuting = false;
        }

        private ScriptCode CreateCode(int index)
        {
            var codeData = regionData.GetCodeData(index);
            
            return codeData == null ? null : new ScriptCode(this, codeData);
        }
        
#if ENABLE_SERIALIZATION
        void ISerializable.Serialize(SerializationStream stream)
        {
            stream.Write(currentCodeIndex);
            stream.Write(IsExecuting);
            stream.Write(IsPaused);
        }

        void ISerializable.Deserialize(SerializationStream stream)
        {
            currentCodeIndex = stream.Read<int>();
            IsExecuting = stream.Read<bool>();
            IsPaused = stream.Read<bool>();

            if (IsExecuting)
                Start(false);
        }
#endif

        private static void RegisterDamnScriptMethods()
        {
            ScriptEngine.AddMethod("ForceNextCode", async (code, arguments) =>
            {
                code.Parent.ForceNextCode();
                return await ScriptEngine.TryExecuteMoreAsync(0, code, arguments, false);
            });
        }
    }   
}
#endif