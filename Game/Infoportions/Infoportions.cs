using System.Collections.Generic;
using Rietmon.Behaviours;
using Rietmon.Serialization;

namespace Rietmon.Game
{
    [StaticSerializable]
    public static class Infoportions
    {
        private static List<string> infoportions = new List<string>();

        public static void AddInfoportion(string name)
        {
            if (HasInfoportion(name))
                return;

            infoportions.Add(name);
        }

        public static bool HasInfoportion(string name) => infoportions.Contains(name);

        public static void RemoveInfoportion(string name) => infoportions.Remove(name);

        public static void Serialize(SerializationStream stream)
        {
            stream.Write(infoportions);
        }

        public static void Deserialize(SerializationStream stream)
        {
            infoportions = stream.Read<List<string>>();
        }
    }
}
