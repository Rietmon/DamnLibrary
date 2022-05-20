#if UNITY_5_3_OR_NEWER 
using System.Collections.Generic;
using System.Threading.Tasks;
using DamnLibrary.Behaviours;
using DamnLibrary.Extensions;
using DamnLibrary.Game;
using UnityEngine;
#pragma warning disable 4014

namespace DamnLibrary.Management
{
    public class WindowsManager : ProtectedSingletonBehaviour<WindowsManager>
    {
        public static WindowsDataProviderType DataProviderType { get; set; } = WindowsDataProviderType.Resources;
        
        public static int OpenedWindowsCount => openedWindows.Count;

        private static readonly List<WindowBehaviour> openedWindows = new List<WindowBehaviour>();

        public static async Task<T> OpenAsync<T>(string windowName, params object[] arguments) =>
            (await OpenAsync(windowName, arguments)).GetComponent<T>();

        public static T Open<T>(string windowName, params object[] arguments) =>
            Open(windowName, arguments).GetComponent<T>();
        
        public static T OpenWithoutAwaiting<T>(string windowName, params object[] arguments) =>
            OpenWithoutAwaiting(windowName, arguments).GetComponent<T>();

        public static async Task<WindowBehaviour> OpenAsync(string windowName, params object[] arguments)
        {
            if (!Instance)
            {
                Debug.LogError(
                    $"[{nameof(WindowsManager)}] ({nameof(OpenAsync)}) Unable to instantiate window because there is no instance of {nameof(WindowsManager)}!");
                return null;
            }
            
            var windowPrefab = await GetPrefabAsync(windowName);
            if (!windowPrefab)
            {
                Debug.LogError(
                    $"[{nameof(WindowsManager)}] ({nameof(OpenAsync)}) Unable to open window, because prefab is equal null!");
                return null;
            }

            var window = PrepareWindow(windowPrefab, windowName, arguments);
            
            await window.OnOpenAsync();

            return window;
        }

        public static WindowBehaviour Open(string windowName, params object[] arguments)
        {
            if (!Instance)
            {
                Debug.LogError(
                    $"[{nameof(WindowsManager)}] ({nameof(Open)}) Unable to instantiate window because there is no instance of {nameof(WindowsManager)}!");
                return null;
            }
            
            var windowPrefab = GetPrefab(windowName);
            if (!windowPrefab)
            {
                Debug.LogError(
                    $"[{nameof(WindowsManager)}] ({nameof(Open)}) Unable to open window, because prefab is equal null!");
                return null;
            }

            var window = PrepareWindow(windowPrefab, windowName, arguments);
            
            window.OnOpenAsync().Wait();

            return window;
        }

        public static WindowBehaviour OpenWithoutAwaiting(string windowName, params object[] arguments)
        {
            if (!Instance)
            {
                Debug.LogError(
                    $"[{nameof(WindowsManager)}] ({nameof(OpenWithoutAwaiting)}) Unable to instantiate window because there is no instance of {nameof(WindowsManager)}!");
                return null;
            }

            var windowPrefab = GetPrefab(windowName);
            if (!windowPrefab)
            {
                Debug.LogError(
                    $"[{nameof(WindowsManager)}] ({nameof(Open)}) Unable to open window, because prefab is equal null!");
                return null;
            }

            var window = PrepareWindow(windowPrefab, windowName, arguments);
            
            window.OnOpenAsync();

            return window;
        }

        public static async Task WaitForClose(WindowBehaviour window) =>
            await TaskUtilities.WaitUntil(() => !openedWindows.Contains(window));

        public static async Task OpenAsyncAndWaitForClose(string windowName, params object[] arguments) =>
            await WaitForClose(await OpenAsync(windowName, arguments));

        public static async Task CloseAsync(string windowName) =>
            await CloseAsync(GetOpenedWindowByName(windowName));

        public static void Close(string windowName) => 
            Close(GetOpenedWindowByName(windowName));

        public static void CloseWithoutAwaiting(string windowName) =>
            CloseWithoutAwaiting(GetOpenedWindowByName(windowName));

        public static async Task CloseAsync(WindowBehaviour window)
        {
            if (window == null)
                return;

            await window.OnCloseAsync();

            openedWindows.Remove(window);

            window.DestroyObject();
        }

        public static void Close(WindowBehaviour window)
        {
            if (window == null)
                return;

            window.OnCloseAsync().Wait();

            openedWindows.Remove(window);

            window.DestroyObject();
        }
        
        public static void CloseWithoutAwaiting(WindowBehaviour window)
        {
            if (window == null)
                return;

            window.OnCloseAsync();

            openedWindows.Remove(window);

            window.DestroyObject();
        }

        public static WindowBehaviour GetOpenedWindowByName(string windowName) =>
            openedWindows.Find((window) => window.WindowName == windowName);
        
        public static T GetOpenedWindowByName<T>(string windowName) =>
            openedWindows.Find((window) => window.WindowName == windowName).GetComponent<T>();

        private static Prefab<WindowBehaviour> GetPrefab(string windowName)
        {
            switch (DataProviderType)
            {
                case WindowsDataProviderType.Resources:
                {
                    return ResourcesManager.GetWindowPrefab(windowName);
                }
#if ENABLE_ADDRESSABLE
                case WindowsDataProviderType.Addressable:
                {
                    return AddressableManager.GetWindowPrefabAsync(windowName).Result;
                }
#endif
            }

            return null;
        }
        
        private static async Task<Prefab<WindowBehaviour>> GetPrefabAsync(string windowName)
        {
            switch (DataProviderType)
            {
                case WindowsDataProviderType.Resources:
                {
                    return await ResourcesManager.GetWindowPrefabAsync(windowName);
                }
#if ENABLE_ADDRESSABLE
                case WindowsDataProviderType.Addressable:
                {
                    return await AddressableManager.GetWindowPrefabAsync(windowName);
                }
#endif
            }

            return null;
        }

        private static WindowBehaviour PrepareWindow(Prefab<WindowBehaviour> windowPrefab, string windowName, object[] arguments)
        {
            var window = windowPrefab.SimpleInstantiate(Instance.transform);
            openedWindows.Add(window);

            window.WindowName = windowName;
            window.Arguments = arguments;

            return window;
        }
    }
}
#endif