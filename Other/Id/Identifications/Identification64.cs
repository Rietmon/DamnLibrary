using Rietmon.Serialization;

namespace Rietmon.Other
{
    internal class Identification64 : Identification
    {
        public override byte Size => 8;

        public override object Id => id;

        public long id;

        public Identification64(long id)
        {
            this.id = id;
        }
        
        protected override bool Compare(Identification other)
        {
            return other switch
            {
                Identification8 id8 => id == id8.id,
                Identification16 id16 => id == id16.id,
                Identification32 id32 => id == id32.id,
                Identification64 id64 => id == id64.id,
                _ => false
            };
        }
    }
}