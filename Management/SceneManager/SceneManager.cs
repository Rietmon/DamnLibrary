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
        
        private static readonly Dictionary<string, AsyncOperation> preloadingScenes = new();

        private static readonly Dictionary<string, AsyncOperation> preloadedScenes = new();
    
        public static void PreloadScene(string name, bool enableActivation = false, Action preloadCallback = null)
        {
            var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
            operation.allowSceneActivation = enableActivation;
        
            preloadingScenes.Add(name, operation);
            
            operation.completed += (_) =>
            {
                preloadCallback?.Invoke();
                preloadingScenes.Remove(name);
                if (!operation.allowSceneActivation)
                    preloadedScenes.Add(name, operation);
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
            !preloadingScenes.ContainsKey(name) && preloadedScenes.ContainsKey(name) || ActiveScene.name == name;
        
        public static bool IsScenePreloading(string name) => preloadingScenes.ContainsKey(name);

        public static float GetScenePreloadingProgress(string name) => preloadingScenes[name].progress;

        public static void EnableScene(string name)
        {
            if (preloadingScenes.TryGetValue(name, out var preloadingOperation))
            {
                preloadingOperation.allowSceneActivation = true;
            }
            else if (preloadedScenes.TryGetValue(name, out var preloadedOperation))
            {
                preloadedOperation.allowSceneActivation = true;
                preloadedScenes.Remove(name);
            }
        }
        
        public static async Task UnloadSceneAsync(string name)
        {
            if (IsScenePreloading(name))
            {
                await TaskUtilities.WaitUntil(() => IsScenePreloaded(name));
                EnableScene(name);
            }
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(name);
            
            preloadingScenes.Remove(name);
        }
        
        public static async Task ChangeSceneAsync(string name)
        {
            await PreloadSceneAsync(name, true);
            
            UnloadSceneAsync(ActiveScene.name);
        }

        public static void LoadScene(string name)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(name);
        }
    }
}
#endif