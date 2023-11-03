#if ENABLE_SERIALIZATION && ENABLE_DAMN_SCRIPT
using System;
using System.Collections;
using System.Collections.Generic;
using DamnLibrary.Behaviours;
using DamnLibrary.DamnScript;
using UnityEngine;

public class DefaultScriptExecutor : DamnBehaviour
{
    /// <summary>
    /// Executor of the script
    /// </summary>
    public ScriptExecutor Executor { get; private set; }

    [SerializeField] private DamnScriptAsset script;

    private void Start()
    {
        Executor = new ScriptExecutor();
        Executor.CreateScriptFromCode(script.name, script.Content);
        Executor.StartRegion();
    }
}
#endif