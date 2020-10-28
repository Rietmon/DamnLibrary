using System.IO;
using UnityEngine;

namespace Rietmon.ResourcesManagement
{
    public static class ResourcesManager
    {
        public static string ScriptsPath => "/Scripts";

        public static byte[] LoadStreamingAsset(string path)
        {
            path = Application.streamingAssetsPath + path;
            if (path.Contains("://"))
            {
                WWW www = new WWW(path);

                while (!www.isDone) ;
            
                return www.bytes;
            }

            return File.ReadAllBytes(path);
        }

        public static void DeleteAll(string path)
        {
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);

            foreach (var file in files)
            {
                File.Delete(file);
            }

            foreach (var directory in directories)
            {
                Directory.Delete(directory);
            }
        }

        public static void VerifyPath(string path)
        {
            void CheckDirectory(string p)
            {
                if (Directory.Exists(p)) return;

                CheckDirectory(Directory.GetDirectoryRoot(p));
                Directory.CreateDirectory(p);
            }

            CheckDirectory(path);
        }

        public static byte[][] LoadScripts()
        {
            var scriptsList = StreamingAssetsList.ScriptsList;

            var result = new byte[scriptsList.Length][];

            for (var i = 0; i < scriptsList.Length; i++)
            {
                result[i] = LoadStreamingAsset($"{ScriptsPath}/{scriptsList[i]}.lua");
            }

            return result;
        }
    }
}
