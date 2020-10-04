using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rietmon.Common.Managment
{
    public static class SceneManager
    {
        private static readonly Dictionary<string, AsyncOperation> scenesToPrepare = new Dictionary<string, AsyncOperation>();
    
        public static void PreloadSceneAsync(string name, Action preloadCallback = null)
        {
            var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
            operation.allowSceneActivation = false;
        
            scenesToPrepare.Add(name, operation);
            
            operation.completed += (result) => preloadCallback?.Invoke();
        }

        public static bool IsScenePreloaded(string name) => scenesToPrepare[name].isDone;

        public static float GetScenePreloadingProgress(string name) => scenesToPrepare[name].progress;
    
        public static void EnablePreloadedScene(string name) => scenesToPrepare[name].allowSceneActivation = true;

        public static void LoadScene(string name) => UnityEngine.SceneManagement.SceneManager.LoadScene(name);

        public static Scene GetActiveScene() => UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        public static void UnloadSceneAsync(string name) => UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(name);
    }
}
