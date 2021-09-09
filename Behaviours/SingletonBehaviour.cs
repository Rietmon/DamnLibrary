#if UNITY_5_3_OR_NEWER 
namespace Rietmon.Behaviours
{
    public class SingletonBehaviour<T> : UnityBehaviour where T : class
    {
        public static T Instance { get; private set; }

        public SingletonBehaviour()
        {
            Instance = this as T;
        }
    }
    
    public class ProtectedSingletonBehaviour<T> : UnityBehaviour where T : class
    {
        protected static T Instance { get; private set; }

        public ProtectedSingletonBehaviour()
        {
            Instance = this as T;
        }
    }
}
#endif