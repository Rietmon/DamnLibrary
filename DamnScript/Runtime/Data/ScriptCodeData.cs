﻿#if ENABLE_DAMN_SCRIPT
namespace DamnLibrary.DamnScript.Runtime.Data
{
    public class ScriptCodeData
    {
        public string[] Codes { get; }

        public ScriptCodeData(string[] codes)
        {
            Codes = codes;
        }
    }
}
#endif