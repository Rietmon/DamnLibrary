#if ENABLE_DAMN_SCRIPT
using System.Threading.Tasks;
using DamnLibrary.DamnScript.Data;

namespace DamnLibrary.DamnScript.Executing
{
    public class ScriptCode
    {
        /// <summary>
        /// Script that contains this code
        /// </summary>
        public Script Script => Parent.Parent;
        
        /// <summary>
        /// Region that contains this code
        /// </summary>
        public ScriptRegion Parent { get; }

        private ScriptCodeData CodeData { get; }

        public ScriptCode(ScriptRegion parent, ScriptCodeData codeData)
        {
            Parent = parent;
            CodeData = codeData;
        }

        /// <summary>
        /// Execute this code async
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ExecuteAsync() => await ScriptEngine.InvokeAsync(this, CodeData.Codes);
    
        /// <summary>
        /// Execute this code
        /// </summary>
        /// <returns></returns>
        public bool Execute() => ScriptEngine.Invoke(this, CodeData.Codes);
    }
  
}
#endif