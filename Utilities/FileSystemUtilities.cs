using System.IO;
using System.Threading.Tasks;
using DamnLibrary.Debugs;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace DamnLibrary.Utilities
{
    public static class FileSystemUtilities
    {
        /// <summary>
        /// Check if a directory exists, if not, create it
        /// </summary>
        /// <param name="path">Path to directory</param>
        public static void CheckOrCreateDirectory(string path)
        {
            if (Path.HasExtension(path))
                path = Path.GetDirectoryName(path);
            
            void CheckDirectory(string p)
            {
                if (Directory.Exists(p)) return;

                CheckDirectory(Directory.GetDirectoryRoot(p));
                Directory.CreateDirectory(p);
            }

            CheckDirectory(path);
        }

        /// <summary>
        /// Write bytes to file async
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="bytes">Bytes to write</param>
        public static async Task WriteAllBytesAsync(string path, byte[] bytes)
        {
            var fileStream = File.Open(path, FileMode.OpenOrCreate);
            await fileStream.WriteAsync(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Read bytes from file async
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>Read bytes</returns>
        public static async Task<byte[]> ReadAllBytesAsync(string path)
        {
            var fileStream = File.OpenRead(path);
            var bytes = new byte[fileStream.Length];
            var readBytes = await fileStream.ReadAsync(bytes, 0, bytes.Length);
            if (readBytes != bytes.Length)
            {
                UniversalDebugger.LogError(
                    $"[{nameof(FileSystemUtilities)}] ({nameof(ReadAllBytesAsync)}) Read {readBytes} bytes, but expected {bytes.Length}");
            }
            return bytes;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Return selected folder in editor
        /// </summary>
        /// <returns>Path to folder</returns>
        public static string Editor_GetSelectedFolder()
        {
            var path = "Assets/";
            var objects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            foreach (var obj in objects)
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }

            return $"{path}/";
        }
#endif
    }
}
