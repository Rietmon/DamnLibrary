#if ENABLE_DAMN_SCRIPT
using DamnLibrary.Games.DamnScript.Runtime.Executing;

namespace DamnLibrary.Games.DamnScript.Runtime
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