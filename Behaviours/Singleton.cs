namespace DamnLibrary.Behaviours
{
    public class Singleton<T> where T : class
    {
        public static T Instance { get; private set; }

        public Singleton()
        {
            Instance = this as T;
        }
    }
    
    public class ProtectedSingleton<T> where T : class
    {
        protected static T Instance { get; private set; }

        public ProtectedSingleton()
        {
            Instance = this as T;
        }
    }
}