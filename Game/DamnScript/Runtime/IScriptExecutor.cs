#if ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript.Executing;

namespace DamnLibrary.DamnScript
{
    public interface IScriptExecutor
    { 
        Script Script { get; }
    } 
}
#endif