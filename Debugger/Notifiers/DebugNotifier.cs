using UnityEngine;

namespace Rietmon.Debugging
{
    public abstract class DebugNotifier
    {
        public virtual void MessageNotify(string condition, string stacktrace, LogType type) { }

        public virtual void GlobalNotify() { }
    }
}