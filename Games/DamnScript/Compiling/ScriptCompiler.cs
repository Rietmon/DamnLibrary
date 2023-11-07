#if ENABLE_SERIALIZATION && ENABLE_DAMN_SCRIPT
using DamnLibrary.Debugs;
using DamnLibrary.Games.DamnScript.Compiling.Containers;
using DamnLibrary.Games.DamnScript.Runtime;
using DamnLibrary.Games.DamnScript.Runtime.Data;

namespace DamnLibrary.Games.DamnScript.Compiling
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
			var debugStopwatch = new DebugStopwatch();
			debugStopwatch.Start();
#endif

			var stream = SerializationManager.CreateSerializationStream();
			var compilingContainer = ScriptDataCompilingContainer.FromData(scriptData);
			stream.Write(compilingContainer);

#if DEBUG
			UniversalDebugger.Log(
				$"[{nameof(ScriptCompiler)}] ({nameof(CompileToBytes)}) Compiled script in {debugStopwatch.Stop()} ms");
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
			var debugStopwatch = new DebugStopwatch();
			debugStopwatch.Start();
#endif

			var stream = SerializationManager.CreateDeserializationStream(bytes);
			var compilingContainer = stream.Read<ScriptDataCompilingContainer>();

#if DEBUG
			UniversalDebugger.Log(
				$"[{nameof(ScriptCompiler)}] ({nameof(DecompileFromBytes)}) Decompiled script in {debugStopwatch.Stop()} ms");
#endif

			return compilingContainer.ToData();
		}
	}
}
#endif