// #if UNITY_5_3_OR_NEWER 
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using DamnLibrary.Extensions;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// #pragma warning disable 4014
//
// namespace DamnLibrary.Management
// {
//     public static class SceneManager
//     {
//         public static Scene ActiveScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene();
//         
//         private static readonly Dictionary<string, AsyncOperation> preloadingScenes = new();
//
//         private static readonly Dictionary<string, AsyncOperation> preloadedScenes = new();
//
//         public static async Task LoadSceneAsync(string name)
//         {
//             var sceneTask = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
//             await TaskUtilities.WaitUntil(() => sceneTask.isDone);
//         }
//
//         public static void PreloadScene(string name, bool enableActivation = false, Action preloadCallback = null)
//         {
//             Debug.Log(11);
//             var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
//             Debug.Log(12);
//             operation.allowSceneActivation = enableActivation;
//             operation.priority = 0xfffffff;
//             Debug.Log(12);
//         
//             preloadingScenes.Add(name, operation);
//             Debug.Log(13);
//
//             async void Test()
//             {
//                 while (true)
//                 {
//                     await TaskUtilities.Yield(5);
//                     Debug.Log(operation.progress + " " + operation.isDone);
//                 }
//             }
//             Test();
//             
//             operation.completed += (_) =>
//             {
//                 preloadCallback?.Invoke();
//                 Debug.Log(21);
//                 preloadingScenes.Remove(name);
//                 Debug.Log(22);
//                 if (!operation.allowSceneActivation)
//                 {
//                     preloadedScenes.Add(name, operation);
//                     Debug.Log(23);
//                 }
//                 Debug.Log(24);
//             };
//             Debug.Log(14);
//         }
//         
//         public static async Task PreloadSceneAsync(string name, bool enableActivation = false)
//         {
//             var isPreloaded = false;
//             void Callback() => isPreloaded = true;
//             
//             PreloadScene(name, enableActivation, Callback);
//
//             if (enableActivation)
//             {
//                 await TaskUtilities.WaitUntil(() => isPreloaded);
//             }
//         }
//
//         public static bool IsScenePreloaded(string name) =>
//             !preloadingScenes.ContainsKey(name) && preloadedScenes.ContainsKey(name) || ActiveScene.name == name;
//         
//         public static bool IsScenePreloading(string name) => preloadingScenes.ContainsKey(name);
//
//         public static float GetScenePreloadingProgress(string name) => preloadingScenes[name].progress;
//
//         public static void EnableScene(string name)
//         {
//             if (preloadingScenes.TryGetValue(name, out var preloadingOperation))
//             {
//                 preloadingOperation.allowSceneActivation = true;
//             }
//             else if (preloadedScenes.TryGetValue(name, out var preloadedOperation))
//             {
//                 preloadedOperation.allowSceneActivation = true;
//                 preloadedScenes.Remove(name);
//             }
//         }
//
//         public static async Task UnloadSceneAsync(string name)
//         {
//             if (IsScenePreloading(name))
//             {
//                 await TaskUtilities.WaitUntil(() => IsScenePreloaded(name));
//                 EnableScene(name);
//             }
//
//             UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(name);
//
//             preloadingScenes.Remove(name);
//         }
//
//         public static void LoadScene(string name)
//         {
//             UnityEngine.SceneManagement.SceneManager.LoadScene(name);
//         }
//     }
// }
// #endif