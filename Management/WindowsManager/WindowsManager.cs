#if UNITY_5_3_OR_NEWER 
using System.Collections.Generic;
using System.Threading.Tasks;
using DamnLibrary.Behaviours;
using DamnLibrary.Debugging;
using DamnLibrary.Extensions;
using DamnLibrary.Game;

#pragma warning disable 4014

namespace DamnLibrary.Management
{
    public class WindowsManager : ProtectedSingletonBehaviour<WindowsManager>
    {
        public static WindowsDataProviderType DataProviderType { get; set; } = WindowsDataProviderType.Resources;
        
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
            window.SetActiveObject(true);
            
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
            
            window.SetActiveObject(true);
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

            window.DestroyObject();
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
                window.Animator.Internal_CloseWindow = () => window.CloseAsync();

            return window;
        }
    }
}
#endif