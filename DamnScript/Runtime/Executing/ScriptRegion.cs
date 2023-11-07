#if ENABLE_DAMN_SCRIPT
using System.Threading.Tasks;
using DamnLibrary.DamnScript.Parsers;
using DamnLibrary.DamnScript.Runtime.Data;
using DamnLibrary.Serializations;
using DamnLibrary.Serializations.Serializables;
using DamnLibrary.Utilities;

namespace DamnLibrary.DamnScript.Runtime.Executing
{
    [DamnScriptable]
    public class ScriptRegion 
#if ENABLE_SERIALIZATION
        : ISerializable
#endif
    {
        /// <summary>
        /// Name of this region
        /// </summary>
        public string Name => RegionData.Name;
        
        /// <summary>
        /// Script that contains this region
        /// </summary>
        public Script Parent { get; }

        /// <summary>
        /// Is nothing more to execute in this region?
        /// </summary>
        public bool IsOver => currentCodeIndex >= RegionData.CodesCount;

        /// <summary>
        /// Is this region paused?
        /// </summary>
        public bool IsPaused { get; private set; }
        
        /// <summary>
        /// Is this region stopped?
        /// </summary>
        public bool IsStopped { get; private set; }
        
        /// <summary>
        /// Is this region executing right now?
        /// </summary>
        public bool IsExecuting { get; private set; }

        private ScriptRegionData RegionData { get; }

        private int currentCodeIndex;

        public ScriptRegion(Script parent, ScriptRegionData regionData)
        {
            Parent = parent;
            RegionData = regionData;
        }

        /// <summary>
        /// Start this region execution in background
        /// </summary>
        /// <param name="resetValues">If true - will reset current index and statuses</param>
        public void Start(bool resetValues = true)
        {
            if (resetValues)
            {
                IsPaused = false;
                IsStopped = false;
                currentCodeIndex = 0;
            }
            Parent.OnRegionStart(this);
            Handler();
        }

        /// <summary>
        /// Resume this region execution
        /// </summary>
        public void Resume() => IsPaused = false;

        /// <summary>
        /// Pause this region execution
        /// </summary>
        public void Pause() => IsPaused = true;

        /// <summary>
        /// Stop this region execution
        /// </summary>
        public void Stop() => IsStopped = true;

        /// <summary>
        /// Skip current code and execute next one
        /// </summary>
        public void ForceNextCode() => currentCodeIndex++;

        private async void Handler()
        {
            IsExecuting = true;

            var stopTask = TaskUtilities.WaitUntil(() => IsStopped);
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
                    IsStopped = false;
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
            var codeData = RegionData.GetCodeData(index);
            
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
            currentCodeIndex = stream.ReadInt();
            IsExecuting = stream.ReadBool();
            IsPaused = stream.ReadBool();

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