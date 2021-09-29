using System.IO;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Rietmon.FileSystem
{
    public static class FileSystemUtilities
    {
        public static void CheckOrCreateDirectory(string directory)
        {
            if (Path.HasExtension(directory))
                directory = Path.GetDirectoryName(directory);
            
            void CheckDirectory(string p)
            {
                if (Directory.Exists(p)) return;

                CheckDirectory(Directory.GetDirectoryRoot(p));
                Directory.CreateDirectory(p);
            }

            CheckDirectory(directory);
        }

        public static async Task WriteAllBytesAsync(string path, byte[] bytes)
        {
            var fileStream = File.Open(path, FileMode.OpenOrCreate);
            await fileStream.WriteAsync(bytes, 0, bytes.Length);
        }

        public static async Task<byte[]> ReadAllBytesAsync(string path)
        {
            var fileStream = File.OpenRead(path);
            var bytes = new byte[fileStream.Length];
            await fileStream.ReadAsync(bytes, 0, bytes.Length);
            return bytes;
        }

#if UNITY_EDITOR
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
