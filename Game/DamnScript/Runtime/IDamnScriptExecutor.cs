#if UNITY_5_3_OR_NEWER  && ENABLE_DAMN_SCRIPT
namespace Rietmon.DS
{
    public interface IDamnScriptExecutor
    { 
        DamnScript Script { get; }
    } 
}
#endif