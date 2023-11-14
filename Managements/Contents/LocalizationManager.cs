#if ENABLE_LOCALIZATIONS && ENABLE_ADDRESSABLE && UNITY_5_3_OR_NEWER
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using DamnLibrary.Debugging;

namespace DamnLibrary.Management.Contents
{
    public static class LocalizationManager
    {
        public static Locale SelectedLocale
        {
            get => LocalizationSettings.SelectedLocale;
            set => LocalizationSettings.SelectedLocale = value;
        }

        public static LocalizedStringDatabase StringDatabase => 
            LocalizationSettings.StringDatabase;

        public static LocalizedAssetDatabase AssetDatabase => 
            LocalizationSettings.AssetDatabase;

        public static async Task<StringTable> GetStringTableAsync(string tableName) =>
            await StringDatabase.GetTableAsync(tableName).Task;

        public static StringTable GetStringTable(string tableName) =>
            StringDatabase.GetTable(tableName);

        public static async Task<string> GetLocalizedStringAsync(string tableName, string key) =>
            await StringDatabase.GetLocalizedStringAsync(tableName, key).Task;

        public static string GetLocalizedString(string tableName, string key) =>
            StringDatabase.GetLocalizedString(tableName, key);

        public static string GetLocalizedString(StringTable table, string key)
        {
            if (table == null)
                return string.Empty;
            
            var result = table.GetEntry(key);
            return result != null ? result.LocalizedValue : $"No translation found for \'{key}\' in {table.name}";
        }

        public static async Task<AssetTable> GetAssetTableAsync(string tableName) =>
            await AssetDatabase.GetTableAsync(tableName).Task;
        
        public static AssetTable GetAssetTable(string tableName) =>
            AssetDatabase.GetTable(tableName);

        public static async Task<T> GetLocalizedAssetAsync<T>(string tableName, string key) where T : Object
        {
            var result = await AssetDatabase.GetLocalizedAssetAsync<T>(tableName, key).Task;
            return result;
        }

        public static T GetLocalizedAsset<T>(string tableName, string key) where T : Object
        {
            var result = AssetDatabase.GetLocalizedAsset<T>(tableName, key);
            return result;
        }

        public static async Task<T> GetLocalizedAssetAsync<T>(AssetTable table, string key) where T : Object
        {
            var result = await table.GetAssetAsync<T>(key).Task;
            return result;
        }

        public static T GetLocalizedAsset<T>(AssetTable table, string key) where T : Object
        {
            var result = table.GetAssetAsync<T>(key).WaitForCompletion();
            return result;
        }
    }
}
#endif