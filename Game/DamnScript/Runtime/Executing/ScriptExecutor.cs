using System.IO;
using Rietmon.DamnScript.Data;
using Rietmon.DamnScript.Executing;
using Rietmon.Serialization;

namespace Rietmon.DamnScript
{
    public class ScriptExecutor : IScriptExecutor, ISerializable
    {
        public Script Script { get; set; }

        public void Begin() => Script.Begin();

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

        void ISerializable.Serialize(SerializationStream stream)
        {
            ((ISerializable)Script).Serialize(stream);
        }

        void ISerializable.Deserialize(SerializationStream stream)
        {
            ((ISerializable)Script).Deserialize(stream);
        }
    }
}