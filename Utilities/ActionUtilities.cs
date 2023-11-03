namespace DamnLibrary.DamnLibrary.Utilities
{
    public static class ActionUtilities
    {
        /// <summary>
        /// Return a dummy action
        /// </summary>
        /// <returns>Dummy action</returns>
        public static Action GetDummy()
        {
            static void Dummy() { }
            return Dummy;
        }
    }
}