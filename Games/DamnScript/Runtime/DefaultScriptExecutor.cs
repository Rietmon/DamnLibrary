#if ENABLE_SERIALIZATION && ENABLE_DAMN_SCRIPT
using DamnLibrary.Behaviours;
using DamnLibrary.Games.DamnScript.UnityAssets;

namespace DamnLibrary.Games.DamnScript.Runtime;

public class DefaultScriptExecutor : DamnBehaviour
{
    /// <summary>
    /// Executor of the script
    /// </summary>
    public ScriptExecutor Executor { get; private set; }

    [SerializeField] private DamnScriptAsset script;

    private void Start()
    {
        Executor = new ScriptExecutor();
        Executor.CreateScriptFromCode(script.name, script.Content);
        Executor.StartRegion();
    }
}
#endif