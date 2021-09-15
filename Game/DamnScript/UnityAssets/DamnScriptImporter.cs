#if UNITY_EDITOR && ENABLE_DAMN_SCRIPT
using System.IO;
using Rietmon.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Rietmon.DamnScript
{
    [ScriptedImporter(1, "ds")]
    public class DamnScriptImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var damnScriptAsset = ScriptableObject.CreateInstance<DamnScriptAsset>();
            damnScriptAsset.Content = File.ReadAllText(ctx.assetPath);

            ctx.AddObjectToAsset(Path.GetFileName(ctx.assetPath), damnScriptAsset);
        }

        [MenuItem("Assets/Create/Damn Script", false, 10)]
        private static void CreateDamnScriptAsset(MenuCommand menuCommand)
        {
            File.Create($"{IOManager.Editor_GetSelectedFolder()}/NewDamnScript.ds").Dispose();
        
            AssetDatabase.Refresh();
        }
    }
}
#endif
