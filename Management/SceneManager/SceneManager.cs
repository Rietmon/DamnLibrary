#if UNITY_5_3_OR_NEWER 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DamnLibrary.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
#pragma warning disable 4014

namespace DamnLibrary.Management
{
    public static class SceneManager
    {
        public static Scene ActiveScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        
        private static readonly Dictionary<string, AsyncOperation> scenesInPreloading = new();
    
        public static void PreloadScene(string name, bool enableActivation = false, Action preloadCallback = null)
        {
            var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
            operation.allowSceneActivation = enableActivation;
        
            scenesInPreloading.Add(name, operation);
            
            operation.completed += (result) =>
            {
                preloadCallback?.Invoke();
                scenesInPreloading.Remove(name);
            };
        }
        
        public static async Task PreloadSceneAsync(string name, bool enableActivation = false)
        {
            var isPreloaded = false;
            void Callback() => isPreloaded = true;
            
            PreloadScene(name, enableActivation, Callback);

            if (enableActivation)
            {
                await TaskUtilities.WaitUntil(() => isPreloaded);
            }
        }
        public static bool IsScenePreloaded(string name) =>
            !scenesInPreloading.ContainsKey(name) || ActiveScene.name == name;
        
        public static bool IsScenePreloading(string name) => scenesInPreloading.ContainsKey(name);

        public static float GetScenePreloadingProgress(string name) => scenesInPreloading[name].progress;
    
        public static void EnablePreloadedScene(string name) => scenesInPreloading[name].allowSceneActivation = true;
        
        public static async Task UnloadSceneAsync(string name)
        {
            if (IsScenePreloading(name))
            {
                await TaskUtilities.WaitUntil(() => IsScenePreloaded(name));
                EnablePreloadedScene(name);
            }
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(name);
            
            scenesInPreloading.Remove(name);
        }
        
        public static async Task ChangeSceneAsync(string name)
        {
            UnloadSceneAsync(ActiveScene.name);
            
            await PreloadSceneAsync(name, true);
        }

        public static void LoadScene(string name)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(name);
        }
    }
}
#endif