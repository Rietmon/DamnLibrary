#if UNITY_5_3_OR_NEWER
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

        private static readonly List<WindowBehaviour> openedWindows = new();

        public static async Task<T> OpenAsync<T>(string windowName, WindowContext windowContext = null) =>
            (await OpenAsync(windowName, windowContext)).GetComponent<T>();

        public static async Task<WindowBehaviour> OpenAsync(string windowName, WindowContext windowContext = null)
        {
            if (!Instance)
            {
                UniversalDebugger.LogError(
                    $"[{nameof(WindowsManager)}] ({nameof(OpenAsync)}) Unable to instantiate window because there is no instance of {nameof(WindowsManager)}!");
                return null;
            }
            
            var windowPrefab = await GetPrefabAsync(windowName);
            if (!windowPrefab)
            {
                UniversalDebugger.LogError(
                    $"[{nameof(WindowsManager)}] ({nameof(OpenAsync)}) Unable to open window, because prefab is equal null!");
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
        
        public static async Task ShowAsync(string windowName) =>
            await ShowAsync(GetOpenedWindowByName(windowName));

        public static async Task ShowAsync(WindowBehaviour window)
        {
            window.SetGameObjectActive(true);
            
            await window.OnShow();
            if (window.Animator)
                await window.Animator.PlayShowAnimationAsync();
            await window.OnShowAnimationOver();
        }
        
        public static async Task HideAsync(string windowName) =>
            await HideAsync(GetOpenedWindowByName(windowName));

        public static async Task HideAsync(WindowBehaviour window)
        {
            await window.OnHide();
            if (window.Animator)
                await window.Animator.PlayHideAnimationAsync();
            await window.OnHideAnimationOver();
            
            window.SetGameObjectActive(true);
        }

        public static async Task WaitForClose(WindowBehaviour window) =>
            await TaskUtilities.WaitUntil(() => !openedWindows.Contains(window));

        public static async Task OpenAsyncAndWaitForClose(string windowName, WindowContext windowContext = null) =>
            await WaitForClose(await OpenAsync(windowName, windowContext));

        public static async Task CloseAsync(string windowName) =>
            await CloseAsync(GetOpenedWindowByName(windowName));

        public static async Task CloseAsync(WindowBehaviour window)
        {
            if (window == null)
                return;

            await window.OnClose();
            if (window.Animator)
                await window.Animator.PlayCloseAnimationAsync();
            await window.OnCloseAnimationOver();

            openedWindows.Remove(window);

            window.DestroyThisGameObject();
        }

        public static WindowBehaviour GetOpenedWindowByName(string windowName) =>
            openedWindows.Find((window) => window.WindowName == windowName);
        
        public static T GetOpenedWindowByName<T>(string windowName) =>
            openedWindows.Find((window) => window.WindowName == windowName).GetComponent<T>();
        
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

        private static WindowBehaviour PrepareWindow(Prefab<WindowBehaviour> windowPrefab, string windowName, WindowContext windowContext)
        {
            var window = windowPrefab.Instantiate(Instance.transform);
            openedWindows.Add(window);

            if (windowContext != null)
                windowContext.OwnerWindow = window;
            
            window.WindowName = windowName;
            window.BaseContext = windowContext;
            if (window.Animator)
                window.Animator.Internal_CloseWindow = () => window.CloseAsync().Forget();

            return window;
        }
    }
}
#endif