namespace Rietmon.Other
{
    internal class Identification8 : Identification
    {
        public override byte Size => 1;

        public override object Id => id;

        public byte id;

        public Identification8(byte id)
        {
            this.id = id;
        }
        
        protected override bool Compare(Identification other)
        {
            return other switch
            {
                Identification8 id8 => id == id8.id,
                _ => false
            };
        }
    }
}