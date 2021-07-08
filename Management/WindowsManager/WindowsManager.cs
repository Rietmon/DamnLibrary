#if UNITY_2020
using System.Collections.Generic;
#if ENABLE_UNI_TASK
using Cysharp.Threading.Tasks;
#else
using System.Threading.Tasks;
using Rietmon.Extensions;
#endif
using Rietmon.Behaviours;
using Rietmon.Management;
using UnityEngine;

namespace Rietmon.Management
{
    public class WindowsManager : SingletonBehaviour<WindowsManager>
    {
        public static int OpenedWindowsCount => openedWindows.Count;

        private static readonly List<WindowBehaviour> openedWindows = new List<WindowBehaviour>();

#if ENABLE_UNI_TASK
        public static async UniTask<T> OpenAsync<T>(string windowName, params object[] arguments) =>
        (await OpenAsync(windowName, arguments)).GetComponent<T>();
#else
        public static async Task<T> OpenAsync<T>(string windowName, params object[] arguments) =>
            (await OpenAsync(windowName, arguments)).GetComponent<T>();
#endif

#if ENABLE_UNI_TASK
        public static async UniTask<WindowBehaviour> OpenAsync(string windowName, params object[] arguments)
#else
        public static async Task<WindowBehaviour> OpenAsync(string windowName, params object[] arguments)
#endif
        {
            var windowPrefab = await ResourcesManager.GetWindowPrefabAsync(windowName);
            if (!windowPrefab)
            {
                Debug.LogError(
                    $"[{nameof(WindowsManager)}] ({nameof(OpenAsync)}) Unable to open window, because prefab is equal null!");
                return null;
            }

            var window = windowPrefab.SimpleInstantiate(Instance.transform);
            openedWindows.Add(window);

            window.WindowName = windowName;
            window.Arguments = arguments;
            await window.OnOpenAsync();

            return window;
        }

#if ENABLE_UNI_TASK
        public static async UniTask WaitForClose(WindowBehaviour window) =>
            await UniTask.WaitUntil(() => !openedWindows.Contains(window));
#else
        public static async Task WaitForClose(WindowBehaviour window) =>
            await TaskUtilities.WaitUntil(() => !openedWindows.Contains(window));
#endif

#if ENABLE_UNI_TASK
        public static async UniTask OpenAsyncAndWaitForClose(string windowName, params object[] arguments) =>
            await WaitForClose(await OpenAsync(windowName, arguments));
#else
        public static async Task OpenAsyncAndWaitForClose(string windowName, params object[] arguments) =>
            await WaitForClose(await OpenAsync(windowName, arguments));
#endif

#if ENABLE_UNI_TASK
        public static async UniTask CloseAsync(string windowName) =>
            await CloseAsync(openedWindows.Find((window) => window.WindowName == windowName));
#else
        public static async Task CloseAsync(string windowName) =>
            await CloseAsync(openedWindows.Find((window) => window.WindowName == windowName));
#endif

#if ENABLE_UNI_TASK
    public static async UniTask CloseAsync(WindowBehaviour window)
#else
        public static async Task CloseAsync(WindowBehaviour window)
#endif
        {
            if (window == null)
                return;

            await window.OnCloseAsync();

            openedWindows.Remove(window);

            window.DestroyObject();
        }
    }
}
#endif