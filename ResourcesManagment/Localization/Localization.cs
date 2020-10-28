#if ENABLE_LOCALIZATION
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Rietmon.ResourcesManagement.Localization
{
    public sealed class Localization 
    {
        public static SystemLanguage Language
        {
            get => GetSystemLanguageByIso(LocalizationSettings.Instance.GetSelectedLocale().Identifier.CultureInfo.TwoLetterISOLanguageName);
            set => LocalizationSettings.Instance.SetSelectedLocale(LocalizationSettings.AvailableLocales.GetLocale(value));
        }
    
        private static readonly Dictionary<string, Dictionary<string, string>> stringsPull = new Dictionary<string, Dictionary<string, string>>();

        public static void LoadStringsToPull(string table, string[] strings, Action callback = null)
        {
            if (!stringsPull.TryGetValue(table, out var pull))
            {
                pull = new Dictionary<string, string>();
                stringsPull.Add(table, pull);
            }

            var receivedCallbacks = 0;
        
            void Callback(string id, string result)
            {
                pull.Add(id, result);

                receivedCallbacks++;
            
                if (receivedCallbacks == strings.Length)
                    callback?.Invoke();
            }

            foreach (var str in strings)
            {
                GetStringAsync(table, str, (a) => Callback(str, a));
            }
        }

        public static string GetStringFromPull(string table, string key) => stringsPull[table][key];

        public static void RemoveStringsFromPull(string table, string[] strings)
        {
            var pull = stringsPull[table];

            foreach (var str in strings)
                pull.Remove(str);
        }

        public static void RemoveStringsTableFromPull(string table) => stringsPull.Remove(table);
    
        public static void GetStringAsync(string table, string key, Action<string> callback = null)
        {
            var operation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(table, key);

            operation.Completed += (a) =>
            {
                callback?.Invoke(a.Result);
            };
        }

        public static SystemLanguage GetSystemLanguageByIso(string iso)
        {
            switch (iso)
            {
                case "ru": return SystemLanguage.Russian;
                case "en": return SystemLanguage.English;
                default: return SystemLanguage.Unknown;
            }
        }
    }
}
#endif