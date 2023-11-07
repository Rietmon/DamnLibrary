#if ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript.Parsers;
using DamnLibrary.DamnScript.Runtime.Executing;
using DamnLibrary.Serializations;
using DamnLibrary.Serializations.Serializables;
using DamnLibrary.Utilities.Extensions;

namespace DamnLibrary.DamnScript.Runtime
{
    [DamnScriptable]
    public class ScriptExecutor : IScriptExecutor
#if ENABLE_SERIALIZATION
        , ISerializable
#endif
    {
        /// <summary>
        /// Script that will be executed
        /// </summary>
        public Script Script { get; private set; }

        /// <summary>
        /// Start executing the region
        /// </summary>
        /// <param name="regionName">Region name</param>
        public void StartRegion(string regionName = "Main") => Script.StartRegion(regionName);

        /// <summary>
        /// Resume executing the region
        /// </summary>
        /// <param name="regionName">Region name</param>
        public void ResumeRegion(string regionName) => Script.ResumeRegion(regionName);

        /// <summary>
        /// Pause executing the region
        /// </summary>
        /// <param name="regionName">Region name</param>
        public void PauseRegion(string regionName) => Script.PauseRegion(regionName);

        /// <summary>
        /// Stop executing the region
        /// </summary>
        /// <param name="regionName">Region name</param>
        public void StopRegion(string regionName) => Script.StopRegion(regionName);

        /// <summary>
        /// Create script from .ds file and set it as the current script
        /// </summary>
        /// <param name="filePath">Path to .ds file</param>
        public virtual void CreateScriptFromFile(string filePath)
        {
            var data = ScriptEngine.CreateDataFromFile(filePath);
            Script = new Script(data, this);
        }

        /// <summary>
        /// Create script from code and set it as the current script
        /// </summary>
        /// <param name="name">Script name</param>
        /// <param name="code">Script code</param>
        public virtual void CreateScriptFromCode(string name, string code)
        {
            var data = ScriptEngine.CreateDataFromCode(name, code);
            Script = new Script(data, this);
        }

#if ENABLE_SERIALIZATION
        /// <summary>
        /// Create script from compiled .dsc file and set it as the current script
        /// </summary>
        /// <param name="filePath">Path to .dsc file</param>
        public virtual void CreateScriptFromCompiledFile(string filePath)
        {
            var data = ScriptEngine.CreateFromCompiledFile(filePath);
            Script = new Script(data, this);
        }

        /// <summary>
        /// Create script from compiled code and set it as the current script
        /// </summary>
        /// <param name="name">Script name</param>
        /// <param name="code">Script code</param>
        public virtual void CreateScriptFromCompiledCode(string name, byte[] code)
        {
            var data = ScriptEngine.CreateDataFromCompiledCode(name, code);
            Script = new Script(data, this);
        }
        
        /// <summary>
        /// Serialize script data to restore it later and keep execution from the same point
        /// </summary>
        /// <param name="stream">SerializationStream</param>
        public void Serialize(SerializationStream stream)
        {
            ((ISerializable)Script).Serialize(stream);
        }

        /// <summary>
        /// Deserialize script data to keep execution from the same point
        /// </summary>
        /// <param name="stream">SerializationStream</param>
        public void Deserialize(SerializationStream stream)
        {
            ((ISerializable)Script).Deserialize(stream);
        }
#endif

        private static void RegisterDamnScriptMethods()
        {
            ScriptEngine.AddMethod("StartRegion", async (code, arguments) =>
            {
                code.Script.StartRegion(arguments.GetObject(0));
                
                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            
            ScriptEngine.AddMethod("ResumeRegion", async (code, arguments) =>
            {
                code.Script.ResumeRegion(arguments.GetObject(0));
                
                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            
            ScriptEngine.AddMethod("PauseRegion", async (code, arguments) =>
            {
                code.Script.PauseRegion(arguments.GetObject(0));
                
                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
            
            ScriptEngine.AddMethod("StopRegion", async (code, arguments) =>
            {
                code.Script.StopRegion(arguments.GetObject(0));
                
                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
        }
    }
}
#endif