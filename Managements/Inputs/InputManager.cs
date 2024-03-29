#if UNITY_5_3_OR_NEWER && !ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using System.Linq;
using DamnLibrary.Behaviours;
using UnityEngine;
#if ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript.Parsers;
using DamnLibrary.DamnScript.Runtime;
#endif

namespace DamnLibrary.Managements.Inputs
{
#if ENABLE_DAMN_SCRIPT
    [DamnScriptable]
#endif
    public class InputManager : DamnBehaviour
    {
        public static bool IsPaused { get; set; }
        
        private static readonly Dictionary<KeyCode, List<Action<KeyCode>>> onKeyDownCallbacks = new();

        private static readonly Dictionary<KeyCode, List<Action<KeyCode>>> onKeyPressCallbacks = new();

        private static readonly Dictionary<KeyCode, List<Action<KeyCode>>> onKeyUpCallbacks = new();

        private static readonly List<KeyCode> keysDown = new();

        private static readonly List<KeyCode> keysPress = new();

        private static readonly List<KeyCode> keysUp = new();

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
            Func<KeyCode, bool, bool> func)
        {
            array.Clear();
            array.AddRange(callbacks.Where(callback => func.Invoke(callback.Key, true))
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

        public static bool GetKeyDown(KeyCode code, bool ignorePause = false) => (!IsPaused || ignorePause) && Input.GetKeyDown(code);

        public static bool GetKey(KeyCode code, bool ignorePause = false) => (!IsPaused || ignorePause) && Input.GetKey(code);

        public static bool GetKeyUp(KeyCode code, bool ignorePause = false) => (!IsPaused || ignorePause) && Input.GetKeyUp(code);

        
#if ENABLE_DAMN_SCRIPT
        private static void RegisterDamnScriptMethods()
        {
            ScriptEngine.AddMethod("OnKeyDown", async (code, arguments) =>
            {
                var targetKey = (KeyCode)Enum.Parse(typeof(KeyCode), arguments.GetObject(0));

                await TaskUtilities.WaitUntil(() => GetKeyDown(targetKey));

                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });

            ScriptEngine.AddMethod("OnKey", async (code, arguments) =>
            {
                var targetKey = (KeyCode)Enum.Parse(typeof(KeyCode), arguments.GetObject(0));

                await TaskUtilities.WaitUntil(() => GetKey(targetKey));

                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });

            ScriptEngine.AddMethod("OnKeyUp", async (code, arguments) =>
            {
                var targetKey = (KeyCode)Enum.Parse(typeof(KeyCode), arguments.GetObject(0));

                await TaskUtilities.WaitUntil(() => GetKeyUp(targetKey));

                return await ScriptEngine.TryExecuteMoreAsync(1, code, arguments);
            });
        }
#endif
    }
}
#endif