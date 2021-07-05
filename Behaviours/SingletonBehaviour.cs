#if UNITY_2020
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
}
#endif