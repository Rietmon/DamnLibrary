#if UNITY_5_3_OR_NEWER 
namespace DamnLibrary.Behaviours
{
    public class SingletonBehaviour<T> : DamnBehaviour where T : class
    {
        public static T Instance { get; private set; }

        public SingletonBehaviour()
        {
            Instance = this as T;
        }
    }
    
    public class ProtectedSingletonBehaviour<T> : DamnBehaviour where T : class
    {
        protected static T Instance { get; private set; }

        public ProtectedSingletonBehaviour()
        {
            Instance = this as T;
        }
    }
}
#endif