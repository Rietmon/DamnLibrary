#if UNITY_2020
using System;
using System.Collections.Generic;
using System.Linq;
#if ENABLE_UNI_TASK
using Cysharp.Threading.Tasks;
#else
#endif
using Rietmon.Behaviours;
#if ENABLE_DAMN_SCRIPT
using Rietmon.DS;
#endif
using Rietmon.Extensions;
using UnityEngine;

namespace Rietmon.Management
{
#if ENABLE_DAMN_SCRIPT
    [DamnScriptable]
#endif
    public class InputManager : UnityBehaviour
    {
        private static readonly Dictionary<KeyCode, List<Action<KeyCode>>> onKeyDownCallbacks =
            new Dictionary<KeyCode, List<Action<KeyCode>>>();

        private static readonly Dictionary<KeyCode, List<Action<KeyCode>>> onKeyPressCallbacks =
            new Dictionary<KeyCode, List<Action<KeyCode>>>();

        private static readonly Dictionary<KeyCode, List<Action<KeyCode>>> onKeyUpCallbacks =
            new Dictionary<KeyCode, List<Action<KeyCode>>>();

        private static readonly List<KeyCode> keysDown = new List<KeyCode>();

        private static readonly List<KeyCode> keysPress = new List<KeyCode>();

        private static readonly List<KeyCode> keysUp = new List<KeyCode>();

        private void Update()
        {
            UpdateKeysArray(keysDown, onKeyDownCallbacks, GetKeyDown);
            UpdateKeysArray(keysPress, onKeyPressCallbacks, GetKey);
            UpdateKeysArray(keysUp, onKeyUpCallbacks, GetKeyUp);

            InvokeCallbacks(keysDown, onKeyDownCallbacks);
            InvokeCallbacks(keysPress, onKeyPressCallbacks);
            InvokeCallbacks(keysUp, onKeyUpCallbacks);
        }

        private static void UpdateKeysArray(List<KeyCode> array, Dictionary<KeyCode, List<Action<KeyCode>>> callbacks,
            Func<KeyCode, bool> func)
        {
            array.Clear();
            array.AddRange(callbacks.Where(callback => func.Invoke(callback.Key))
                .Select(callback => callback.Key));
        }

        private static void InvokeCallbacks(List<KeyCode> array, Dictionary<KeyCode, List<Action<KeyCode>>> callbacks)
        {
            foreach (var key in array)
            {
                if (callbacks.TryGetValue(key, out var callbacksArray))
                {
                    foreach (var callback in callbacksArray)
                        callback?.Invoke(key);
                }
            }
        }

        public static void RegisterOnKeyDownCallback(KeyCode keyCode, Action<KeyCode> callback) =>
            RegisterCallback(onKeyDownCallbacks, keyCode, callback);

        public static void UnregisterOnKeyDownCallback(KeyCode keyCode, Action<KeyCode> callback) =>
            UnregisterCallback(onKeyDownCallbacks, keyCode, callback);

        public static void RegisterOnKeyPressCallback(KeyCode keyCode, Action<KeyCode> callback) =>
            RegisterCallback(onKeyPressCallbacks, keyCode, callback);

        public static void UnregisterOnKeyPressCallback(KeyCode keyCode, Action<KeyCode> callback) =>
            UnregisterCallback(onKeyPressCallbacks, keyCode, callback);

        public static void RegisterOnKeyUpCallback(KeyCode keyCode, Action<KeyCode> callback) =>
            RegisterCallback(onKeyUpCallbacks, keyCode, callback);

        public static void UnregisterOnKeyUpCallback(KeyCode keyCode, Action<KeyCode> callback) =>
            UnregisterCallback(onKeyUpCallbacks, keyCode, callback);

        public static void RegisterOnKeyDownAndOnKeyUpCallback(KeyCode keyCode, Action<KeyCode> downCallback,
            Action<KeyCode> upCallback)
        {
            RegisterCallback(onKeyDownCallbacks, keyCode, downCallback);
            RegisterCallback(onKeyUpCallbacks, keyCode, upCallback);
        }

        public static void UnregisterOnKeyDownAndOnKeyUpCallback(KeyCode keyCode, Action<KeyCode> downCallback,
            Action<KeyCode> upCallback)
        {
            UnregisterCallback(onKeyDownCallbacks, keyCode, downCallback);
            UnregisterCallback(onKeyUpCallbacks, keyCode, upCallback);
        }

        private static void RegisterCallback(Dictionary<KeyCode, List<Action<KeyCode>>> keyCallbacks, KeyCode keyCode,
            Action<KeyCode> callback)
        {
            if (!keyCallbacks.TryGetValue(keyCode, out var callbacks))
            {
                callbacks = new List<Action<KeyCode>>();
                keyCallbacks.Add(keyCode, callbacks);
            }

            callbacks.Add(callback);
        }

        private static void UnregisterCallback(Dictionary<KeyCode, List<Action<KeyCode>>> keyCallbacks, KeyCode keyCode,
            Action<KeyCode> callback)
        {
            if (!keyCallbacks.TryGetValue(keyCode, out var callbacks))
                return;

            callbacks.Remove(callback);

            if (callbacks.Count == 0)
                keyCallbacks.Remove(keyCode);
        }

        public static bool GetKeyDown(KeyCode code) => Input.GetKeyDown(code);

        public static bool GetKey(KeyCode code) => Input.GetKey(code);

        public static bool GetKeyUp(KeyCode code) => Input.GetKeyUp(code);

        
#if ENABLE_DAMN_SCRIPT
        private static void RegisterDamnScriptMethods()
        {
            DamnScriptEngine.RegisterMethod("OnKeyDown", async (code, arguments) =>
            {
                var targetKey = (KeyCode)Enum.Parse(typeof(KeyCode), arguments.GetArgument(0));

#if ENABLE_UNI_TASK
            await UniTask.WaitUntil(() => GetKeyDown(targetKey));
#else
                await TaskUtilities.WaitUntil(() => GetKeyDown(targetKey));
#endif

                return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });

            DamnScriptEngine.RegisterMethod("OnKey", async (code, arguments) =>
            {
                var targetKey = (KeyCode)Enum.Parse(typeof(KeyCode), arguments.GetArgument(0));

#if ENABLE_UNI_TASK
            await UniTask.WaitUntil(() => GetKey(targetKey));
#else
                await TaskUtilities.WaitUntil(() => GetKey(targetKey));
#endif

                return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });

            DamnScriptEngine.RegisterMethod("OnKeyUp", async (code, arguments) =>
            {
                var targetKey = (KeyCode)Enum.Parse(typeof(KeyCode), arguments.GetArgument(0));

#if ENABLE_UNI_TASK
            await UniTask.WaitUntil(() => GetKeyUp(targetKey));
#else
                await TaskUtilities.WaitUntil(() => GetKeyUp(targetKey));
#endif

                return await DamnScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
        }
#endif
    }
}
#endif