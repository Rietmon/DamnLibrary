namespace DamnLibrary.Behaviours
{
    public abstract class Singleton<T> where T : class
    {
        public static T Instance { get; private set; }

        protected Singleton()
        {
            Instance = this as T;
        }
    }
    
    public abstract class ProtectedSingleton<T> where T : class
    {
        protected static T Instance { get; private set; }

        protected ProtectedSingleton()
        {
            Instance = this as T;
        }
    }
}