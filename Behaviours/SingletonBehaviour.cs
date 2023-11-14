#if UNITY_5_3_OR_NEWER 
namespace DamnLibrary.Behaviours
{
    public abstract class SingletonBehaviour<T> : DamnBehaviour where T : class
    {
        public static T Instance { get; private set; }

        protected SingletonBehaviour()
        {
            Instance = this as T;
        }
    }
    
    public abstract class ProtectedSingletonBehaviour<T> : DamnBehaviour where T : class
    {
        protected static T Instance { get; private set; }

        protected ProtectedSingletonBehaviour()
        {
            Instance = this as T;
        }
    }
}
#endif