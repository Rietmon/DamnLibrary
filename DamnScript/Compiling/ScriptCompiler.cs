#if ENABLE_SERIALIZATION && ENABLE_DAMN_SCRIPT
using System.Diagnostics;
using System.IO;
using DamnLibrary.DamnScript.Compiling.Containers;
using DamnLibrary.DamnScript.Runtime;
using DamnLibrary.DamnScript.Runtime.Data;
using DamnLibrary.Debugs;
using DamnLibrary.Serializations;

namespace DamnLibrary.DamnScript.Compiling
{
	public static class ScriptCompiler
	{
		/// <summary>
		/// Compile .ds file to .dsc file
		/// </summary>
		/// <param name="inputFile">Path to .ds file</param>
		/// <param name="outputFile">Path to .dsc file</param>
		public static void CompileFromFileToFile(string inputFile, string outputFile = null)
		{
			outputFile ??= inputFile + "c";

			var scriptData = ScriptEngine.CreateDataFromFile(inputFile);

			CompileToFile(scriptData, outputFile);
		}

		/// <summary>
		/// Compile .ds file to bytes
		/// </summary>
		/// <param name="inputFile">Path to .ds file</param>
		/// <returns>Compiled bytes of .dsc</returns>
		public static byte[] CompileFromFileToBytes(string inputFile)
		{
			var scriptData = ScriptEngine.CreateDataFromFile(inputFile);

			return CompileToBytes(scriptData);
		}

		/// <summary>
		/// Compile ScriptData to .dsc file
		/// </summary>
		/// <param name="scriptData">ScriptData</param>
		/// <param name="path">Path to .dsc file</param>
		public static void CompileToFile(ScriptData scriptData, string path)
		{
			var bytes = CompileToBytes(scriptData);
			File.WriteAllBytes(path, bytes);
		}

		public static byte[] CompileToBytes(ScriptData scriptData)
		{
#if DEBUG
			var debugStopwatch = new Stopwatch();
			debugStopwatch.Start();
#endif

			var stream = new SerializationStream();
			var compilingContainer = ScriptDataCompilingContainer.FromData(scriptData);
			stream.Write(compilingContainer);

#if DEBUG
			debugStopwatch.Stop();
			UniversalDebugger.Log(
				$"[{nameof(ScriptCompiler)}] ({nameof(CompileToBytes)}) Compiled script in {debugStopwatch.ElapsedMilliseconds} ms");
#endif

			return stream.ToArray();
		}

		/// <summary>
		/// Decompile .dsc file to ScriptData
		/// </summary>
		/// <param name="filePath">Path to .dsc file</param>
		/// <returns>ScriptData</returns>
		public static ScriptData DecompileFromFile(string filePath)
		{
			var bytes = File.ReadAllBytes(filePath);
			return DecompileFromBytes(bytes);
		}

		/// <summary>
		/// Decompile bytes of .dsc file to ScriptData
		/// </summary>
		/// <param name="bytes">Bytes of .dsc file</param>
		/// <returns>ScriptData</returns>
		public static ScriptData DecompileFromBytes(byte[] bytes)
		{
#if DEBUG
			var debugStopwatch = new Stopwatch();
			debugStopwatch.Start();
#endif

			var stream = new SerializationStream(bytes);
			var compilingContainer = stream.ReadSerializable<ScriptDataCompilingContainer>();

#if DEBUG
			UniversalDebugger.Log(
				$"[{nameof(ScriptCompiler)}] ({nameof(DecompileFromBytes)}) Decompiled script in {debugStopwatch.Stop()} ms");
#endif

			return compilingContainer.ToData();
		}
	}
}
#endif