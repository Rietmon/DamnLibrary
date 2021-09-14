namespace Rietmon.Other
{
    internal class Identification32 : Identification
    {
        public override byte Size => 4;

        public override object Id => id;

        public int id;

        public Identification32(int id)
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
                _ => false
            };
        }
    }
}