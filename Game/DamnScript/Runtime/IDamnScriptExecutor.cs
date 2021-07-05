#if UNITY_2020 && ENABLE_DAMN_SCRIPT
namespace Rietmon.DS
{
    public interface IDamnScriptExecutor
    { 
        DamnScript Script { get; }
    } 
}
#endif