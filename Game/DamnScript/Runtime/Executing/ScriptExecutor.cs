using System.IO;
using Rietmon.DamnScript.Data;
using Rietmon.DamnScript.Executing;
using Rietmon.Extensions;
#if ENABLE_SERIALIZATION
using Rietmon.Serialization;
#endif

namespace Rietmon.DamnScript
{
    [DamnScriptable, DontCreateInstanceAtDeserialization]
    public class ScriptExecutor : IScriptExecutor, ISerializable
    {
        public Script Script { get; private set; }

        public void StartRegion(string regionName = "Main") => Script.StartRegion(regionName);

        public void ResumeRegion(string regionName) => Script.ResumeRegion(regionName);

        public void PauseRegion(string regionName) => Script.PauseRegion(regionName);

        public void StopRegion(string regionName) => Script.StopRegion(regionName);

        public virtual void CreateScriptFromFile(string filePath)
        {
            var data = ScriptEngine.CreateDataFromFile(filePath);
            Script = new Script(data, this);
        }

        public virtual void CreateScriptFromCode(string name, string code)
        {
            var data = ScriptEngine.CreateDataFromCode(name, code);
            Script = new Script(data, this);
        }

#if ENABLE_SERIALIZATION
        public virtual void CreateScriptFromCompiledFile(string filePath)
        {
            var data = ScriptEngine.CreateFromCompiledFile(filePath);
            Script = new Script(data, this);
        }

        public virtual void CreateScriptFromCompiledCode(string name, byte[] code)
        {
            var data = ScriptEngine.CreateDataFromCompiledCode(name, code);
            Script = new Script(data, this);
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

        public void Serialize(SerializationStream stream)
        {
            ((ISerializable)Script).Serialize(stream);
        }

        public void Deserialize(SerializationStream stream)
        {
            ((ISerializable)Script).Deserialize(stream);
        }
    }
}