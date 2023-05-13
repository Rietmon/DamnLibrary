namespace DamnLibrary.Types
{
    internal class Identification16 : Identification
    {
        public override byte Size => 2;

        public override object Id => id;

        public readonly short id;

        public Identification16(short id)
        {
            this.id = id;
        }
        
        protected override bool Compare(Identification other)
        {
            return other switch
            {
                Identification8 id8 => id == id8.id,
                Identification16 id16 => id == id16.id,
                _ => false
            };
        }
    }
}