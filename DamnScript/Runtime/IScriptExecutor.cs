#if ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript.Runtime.Executing;

namespace DamnLibrary.DamnScript.Runtime
{
    public interface IScriptExecutor
    { 
        /// <summary>
        /// Script that will be executed
        /// </summary>
        Script Script { get; }
    } 
}
#endif