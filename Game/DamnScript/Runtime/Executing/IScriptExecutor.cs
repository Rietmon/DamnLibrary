using Rietmon.DamnScript.Executing;

#if ENABLE_DAMN_SCRIPT
namespace Rietmon.DamnScript
{
    public interface IScriptExecutor
    { 
        Script Script { get; }
    } 
}
#endif