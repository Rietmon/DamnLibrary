using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Rietmon.Behaviours;
using UnityEngine;

public class WindowsManager : SingletonBehaviour<WindowsManager>
{
    public static int OpenedWindowsCount => openedWindows.Count;
    
    private static readonly List<WindowBehaviour> openedWindows = new List<WindowBehaviour>();

    public static async UniTask<T> OpenAsync<T>(string windowName, params object[] arguments) =>
        (await OpenAsync(windowName, arguments)).GetComponent<T>();

    public static async UniTask<WindowBehaviour> OpenAsync(string windowName, params object[] arguments)
    {
        var windowPrefab = await BaseResourcesManager.GetWindowPrefabAsync(windowName);
        if (!windowPrefab)
        {
            Debug.LogError($"[{nameof(WindowsManager)}] ({nameof(OpenAsync)}) Unable to open window, because prefab is equal null!");
            return null;
        }

        var window = windowPrefab.SimpleInstantiate(Instance.transform);
        openedWindows.Add(window);

        window.WindowName = windowName;
        window.Arguments = arguments;
        await window.OnOpenAsync();

        return window;
    }

    public static async UniTask WaitForClose(WindowBehaviour window) =>
        await UniTask.WaitUntil(() => !openedWindows.Contains(window));

    public static async UniTask OpenAsyncAndWaitForClose(string windowName, params object[] arguments) =>
        await WaitForClose(await OpenAsync(windowName, arguments));

    public static async UniTask CloseAsync(string windowName) =>
        await CloseAsync(openedWindows.Find((window) => window.WindowName == windowName));

    public static async UniTask CloseAsync(WindowBehaviour window)
    {
        if (window == null)
            return;
        
        await window.OnCloseAsync();

        openedWindows.Remove(window);
        
        window.DestroyObject();
    }
}
