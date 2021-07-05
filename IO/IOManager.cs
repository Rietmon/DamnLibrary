using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_2020
using UnityEngine;
#endif

namespace Rietmon.IO
{
    public static class IOManager
    {
        public static void CheckOrCreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                var previousDirectory = Directory.GetParent(directory).FullName;
                CheckOrCreateDirectory(previousDirectory);
                Directory.CreateDirectory(directory);
            }
        }

#if UNITY_EDITOR
        public static string Editor_GetSelectedFolder()
        {
            var path = "Assets/";
            var objects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            foreach (Object obj in objects)
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
