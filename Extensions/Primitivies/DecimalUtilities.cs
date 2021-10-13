namespace Rietmon.Extensions
{
    // Copied from https://stackoverflow.com/questions/3467893/how-do-i-convert-byte-values-into-decimals
    public static class DecimalUtilities
    {
        public static decimal FromBytes(byte[] bytes)
        {
            var bits = new int[4];
            bits[0] = bytes[0] | (bytes[1] << 8) | (bytes[2] << 0x10) | (bytes[3] << 0x18); //lo
            bits[1] = bytes[4] | (bytes[5] << 8) | (bytes[6] << 0x10) | (bytes[7] << 0x18); //mid
            bits[2] = bytes[8] | (bytes[9] << 8) | (bytes[10] << 0x10) | (bytes[11] << 0x18); //hi
            bits[3] = bytes[12] | (bytes[13] << 8) | (bytes[14] << 0x10) | (bytes[15] << 0x18); //flags

            return new decimal(bits);
        }

        public static byte[] GetBytes(decimal value)
        {
            var bytes = new byte[16];

            var bits = decimal.GetBits(value);
            var lo = bits[0];
            var mid = bits[1];
            var hi = bits[2];
            var flags = bits[3];

            bytes[0] = (byte)lo;
            bytes[1] = (byte)(lo >> 8);
            bytes[2] = (byte)(lo >> 0x10);
            bytes[3] = (byte)(lo >> 0x18);
            bytes[4] = (byte)mid;
            bytes[5] = (byte)(mid >> 8);
            bytes[6] = (byte)(mid >> 0x10);
            bytes[7] = (byte)(mid >> 0x18);
            bytes[8] = (byte)hi;
            bytes[9] = (byte)(hi >> 8);
            bytes[10] = (byte)(hi >> 0x10);
            bytes[11] = (byte)(hi >> 0x18);
            bytes[12] = (byte)flags;
            bytes[13] = (byte)(flags >> 8);
            bytes[14] = (byte)(flags >> 0x10);
            bytes[15] = (byte)(flags >> 0x18);

            return bytes;
        }
    }
}