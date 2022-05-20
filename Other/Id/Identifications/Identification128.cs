namespace DamnLibrary.Other
{
    internal class Identification128 : Identification
    {
        public override byte Size => 16;

        public override object Id => id;

        public readonly decimal id;

        public Identification128(decimal id)
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
                Identification128 id128 => id == id128.id,
                _ => false
            };
        }
    }
}