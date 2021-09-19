#if ENABLE_DAMN_SCRIPT
using Rietmon.DamnScript.Executing;

namespace Rietmon.DamnScript
{
    public interface IScriptExecutor
    { 
        Script Script { get; }
    } 
}
#endif