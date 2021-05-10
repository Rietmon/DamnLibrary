namespace Rietmon.Behaviours
{
    public class SingletonBehaviour<T> : UnityBehaviour where T : class
    {
        public static T Instance { get; protected set; }

        public SingletonBehaviour()
        {
            Instance = this as T;
        }
    }
}