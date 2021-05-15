using System.Collections;
using System.Collections.Generic;
using Rietmon.Debugging;
using UnityEngine;

public class SoundNotifier : DebugNotifier
{
    private string[] soundsToNotify;
    
    public override void MessageNotify(string condition, string stacktrace, LogType type)
    {
        
    }
}
