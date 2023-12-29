using UnityEngine;

namespace DamnLibrary.Natives.Android
{
    public static class KeyStore
    {
#if ENABLE_ANDROID && !UNITY_EDITOR
        private static readonly AndroidJavaClass keyStoreClass = new("android.security.KeyChain");
#endif
        
        public static void WriteToKeystore(string alias, string value)
        {
#if ENABLE_ANDROID && !UNITY_EDITOR
            using var keyStore = keyStoreClass.CallStatic<AndroidJavaObject>("createInstallIntent");
            using var editor = keyStoreClass.CallStatic<AndroidJavaObject>("getEditor", keyStore);
            editor.Call("putString", alias, value);
            editor.Call("apply");
#else
            PlayerPrefs.SetString(alias, value);
#endif
        }
        
        public static string ReadFromKeystore(string alias)
        {
#if ENABLE_ANDROID && !UNITY_EDITOR
            using var keyStore = keyStoreClass.CallStatic<AndroidJavaObject>("createInstallIntent");
            using var sharedPreferences = keyStoreClass.CallStatic<AndroidJavaObject>("getDefaultSharedPreferences", keyStore);
            return sharedPreferences.Call<string>("getString", alias, null);
#else
            return PlayerPrefs.GetString(alias);
#endif
        }
    }
}