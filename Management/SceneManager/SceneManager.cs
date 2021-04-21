using System;
using System.Collections.Generic;
#if ENABLE_UNI_TASK
using Cysharp.Threading.Tasks;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rietmon.Management
{
    public static class SceneManager
    {
        public static Scene ActiveScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        
        private static readonly Dictionary<string, AsyncOperation> scenesInPreloading = new Dictionary<string, AsyncOperation>();
    
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
        
#if ENABLE_UNI_TASK
        public static async UniTask PreloadSceneAsync(string name, bool enableActivation = false)
        {
            var isPreloaded = false;
            void Callback() => isPreloaded = true;
            
            PreloadScene(name, enableActivation, Callback);

            if (enableActivation)
                await UniTask.WaitUntil(() => isPreloaded);
        }
#endif
        public static bool IsScenePreloaded(string name) =>
            !scenesInPreloading.ContainsKey(name) || ActiveScene.name == name;
        
        public static bool IsScenePreloading(string name) => scenesInPreloading.ContainsKey(name);

        public static float GetScenePreloadingProgress(string name) => scenesInPreloading[name].progress;
    
        public static void EnablePreloadedScene(string name) => scenesInPreloading[name].allowSceneActivation = true;
        
#if ENABLE_UNI_TASK
        public static async void UnloadSceneAsync(string name)
        {
            if (IsScenePreloading(name))
            {
                await UniTask.WaitUntil(() => IsScenePreloaded(name));
                EnablePreloadedScene(name);
            }
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(name);
            
            scenesInPreloading.Remove(name);
        }
#endif
        
#if ENABLE_UNI_TASK
        public static async void ChangeSceneAsync(string name)
        {
            UnloadSceneAsync(ActiveScene.name);
            
            await PreloadSceneAsync(name, true);
        }
#endif
    }
}
