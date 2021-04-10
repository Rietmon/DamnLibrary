using System.Collections.Generic;
using Rietmon.Behaviours;
using Rietmon.Serialization;

namespace Rietmon.Game
{
    public class Infoportions : UnityBehaviour, ISerializable
    {
        public static Infoportions Instance { get; private set; }

        private static List<string> infoportions = new List<string>();
    
        private void Awake()
        {
            Instance = this;
        }

        public static void AddInfoportion(string name)
        {
            if (HasInfoportion(name))
                return;

            infoportions.Add(name);
        }

        public static bool HasInfoportion(string name) => infoportions.Contains(name);

        public static void RemoveInfoportion(string name) => infoportions.Remove(name);

        public void Serialize(SerializationStream stream)
        {
            stream.Write(infoportions);
        }

        public void Deserialize(SerializationStream stream)
        {
            infoportions = stream.Read<List<string>>();
        }
    }
}
