#if ENABLE_DAMN_SCRIPT
using DamnLibrary.DamnScript.Executing;

namespace DamnLibrary.DamnScript
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