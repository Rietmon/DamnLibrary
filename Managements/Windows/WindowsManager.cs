#if UNITY_5_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DamnLibrary.Behaviours;
using DamnLibrary.Debugs;
using DamnLibrary.Games;
using DamnLibrary.Managements.Contents;
using DamnLibrary.Utilities;
using DamnLibrary.Utilities.Extensions;

namespace DamnLibrary.Managements.Windows
{
    public class WindowsManager : ProtectedSingletonBehaviour<WindowsManager>
    {
#if ENABLE_ADDRESSABLE
        public static WindowsDataProviderType DataProviderType { get; set; } = WindowsDataProviderType.Addressable;
#else
        public static WindowsDataProviderType DataProviderType { get; set; } = WindowsDataProviderType.Resources;
#endif
        
        public static int OpenedWindowsCount => openedWindows.Count;

        private static readonly List<Internal_Window> openedWindows = new();

        public static async Task<T> OpenAsync<T>(WindowContext windowContext) where T : Internal_Window
        {
            if (!Instance)
            {
                UniversalDebugger.LogError($"[{nameof(WindowsManager)}] ({nameof(OpenAsync)}) " +
                                           $"Unable to instantiate window because there is no instance of {nameof(WindowsManager)}!");
                return null;
            }

            var windowName = typeof(T).Name;
            var windowPrefab = await GetPrefabAsync<T>(windowName);
            if (!windowPrefab)
            {
                UniversalDebugger.LogError(
                    $"[{nameof(WindowsManager)}] ({nameof(OpenAsync)}) " +
                    $"Unable to open window, because prefab is equal null!");
                return null;
            }

            var window = PrepareWindow(windowPrefab, windowName, windowContext);

            await window.OnOpen();
            if (window.Animator)
                await window.Animator.PlayOpenAnimationAsync();
            await window.OnOpenAnimationOver();
            UniversalDebugger.Log($"[{nameof(WindowsManager)}] ({nameof(OpenAsync)}) Opened {windowName}");

            return window;
        }
        
        public static async Task ShowAsync<T>() =>
            await ShowAsync(GetOpenedWindowByName(typeof(T).Name));

        public static async Task ShowAsync<T>(T internalWindow) where T : Internal_Window
        {
            internalWindow.SetGameObjectActive(true);
            
            await internalWindow.OnShow();
            if (internalWindow.Animator)
                await internalWindow.Animator.PlayShowAnimationAsync();
            await internalWindow.OnShowAnimationOver();
        }
        
        public static async Task HideAsync<T>() =>
            await HideAsync(GetOpenedWindowByName(typeof(T).Name));

        public static async Task HideAsync(Internal_Window internalWindow)
        {
            await internalWindow.OnHide();
            if (internalWindow.Animator)
                await internalWindow.Animator.PlayHideAnimationAsync();
            await internalWindow.OnHideAnimationOver();
            
            internalWindow.SetGameObjectActive(true);
        }

        public static async Task WaitForClose<T>(T internalWindow) where T : Internal_Window =>
            await TaskUtilities.WaitUntil(() => !openedWindows.Contains(internalWindow));

        public static async Task OpenAsyncAndWaitForClose<T>(WindowContext windowContext) where T : Internal_Window =>
            await WaitForClose(await OpenAsync<T>(windowContext));

        public static async Task CloseAsync<T>() where T : Internal_Window =>
            await CloseAsync(GetOpenedWindowByName<T>());

        public static async Task CloseAsync<T>(T internalWindow) where T : Internal_Window
        {
            if (internalWindow == null)
                return;

            await internalWindow.OnClose();
            if (internalWindow.Animator)
                await internalWindow.Animator.PlayCloseAnimationAsync();
            await internalWindow.OnCloseAnimationOver();

            openedWindows.Remove(internalWindow);

            internalWindow.DestroyThisGameObject();
        }

        public static Internal_Window GetOpenedWindowByName(string windowName) =>
            openedWindows.Find((window) => window.WindowName == windowName);

        public static T GetOpenedWindowByName<T>() where T : Internal_Window
        {
            var windowName = typeof(T).Name;
            return (T)openedWindows.Find((window) => window.WindowName == windowName);
        }

        private static async Task<Prefab<T>> GetPrefabAsync<T>(string windowName) where T : Internal_Window =>
            DataProviderType switch
            {
                WindowsDataProviderType.Resources => await ResourcesManager.GetWindowPrefabAsync<T>(windowName),
#if ENABLE_ADDRESSABLE
                WindowsDataProviderType.Addressable => await AddressableManager.GetWindowPrefabAsync<T>(windowName),
#endif
                _ => null
            };

        private static T PrepareWindow<T>(Prefab<T> windowPrefab, string windowName, WindowContext windowContext) 
            where T : Internal_Window
        {
            var window = windowPrefab.Instantiate(Instance.transform);
            openedWindows.Add(window);

            if (windowContext != null)
                windowContext.Owner = window;
            
            window.WindowName = windowName;
            window.BaseContext = windowContext;
            if (window.Animator)
                window.Animator.Internal_CloseWindow = () => window.CloseAsync().Forget();

            return window;
        }
    }
}
#endif