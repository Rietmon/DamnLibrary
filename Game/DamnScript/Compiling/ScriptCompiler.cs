#if ENABLE_SERIALIZATION && ENABLE_DAMN_SCRIPT
using System.IO;
using Rietmon.DamnScript.Data;
using Rietmon.Debugging;
using Rietmon.Serialization;

namespace Rietmon.DamnScript.Compiling
{
    public static class ScriptCompiler
    {
        public static void CompileToFile(ScriptData scriptData, string path)
        {
            var bytes = CompileToBytes(scriptData);
            File.WriteAllBytes(path, bytes);
        }
        public static byte[] CompileToBytes(ScriptData scriptData)
        {
            var debugStopwatch = new DebugStopwatch();
            debugStopwatch.Start();
            
            var stream = SerializationManager.CreateSerializationStream();
            var compilingContainer = ScriptDataCompilingContainer.FromData(scriptData);
            stream.Write(compilingContainer);
            
            debugStopwatch.Stop("Compiled script in {0} ms.");
            
            return stream.ToArray();
        }

        public static ScriptData DecompileFromFile(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            return DecompileFromBytes(bytes);
        }
        public static ScriptData DecompileFromBytes(byte[] bytes)
        {
            var debugStopwatch = new DebugStopwatch();
            debugStopwatch.Start();
            
            var stream = SerializationManager.CreateDeserializationStream(bytes);
            var compilingContainer = stream.Read<ScriptDataCompilingContainer>();
            
            debugStopwatch.Stop("Decompiled script in {0} ms.");
            
            return compilingContainer.ToData();
        }
    }
}
#endif