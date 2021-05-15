using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Id
{
    private Internal_Id UsedId
    {
        get
        {
            if (id8 != null)
                return id8;
            else if (id16 != null)
                return id16;
            else if (id32 != null)
                return id32;
            else if (id64 != null)
                return id64;
            else if (id128 != null)
                return id128;

            Debug.LogError($"[{nameof(Id)}] ({nameof(UsedId)}) No one internal id dont have value!");
            return null;
        }
    }
    
    private Internal_Id8 id8;
    private Internal_Id16 id16;
    private Internal_Id32 id32;
    private Internal_Id64 id64;
    private Internal_Id128 id128;

    public static Id Create8(byte value) =>
        new Id
        {
            id8 = new Internal_Id8(value)
        };
    
    public static Id Create8() =>
        Create8(RandomUtilities.RandomByte);
    
    public static Id Create16(short value) =>
        new Id
        {
            id16 = new Internal_Id16(value)
        };
    
    public static Id Create16() =>
        Create16(RandomUtilities.RandomShort);

    public static Id Create32(int value) =>
        new Id
        {
            id32 = new Internal_Id32(value)
        };
    
    public static Id Create32() =>
        Create32(RandomUtilities.RandomInt);
    
    public static Id Create64(long value) =>
        new Id
        {
            id64 = new Internal_Id64(value)
        };
    
    public static Id Create64() =>
        Create64(RandomUtilities.RandomLong);
    
    public static Id Create128(decimal value) =>
        new Id
        {
            id128 = new Internal_Id128(value)
        };
    
    public static Id Create128() =>
        Create128(RandomUtilities.RandomDecimal);

    public static bool operator ==(Id first, Id second)
    {
        if (Equals(first, null) && Equals(second, null)) return true;
        if (Equals(first, null)) return false;
        if (Equals(second, null)) return false;
        return first.UsedId == second.UsedId;
    }

    public static bool operator !=(Id first, Id second)
    {
        return !(first == second);
    }

    private abstract class Internal_Id
    {
        public readonly byte size;
        
        public readonly object id;

        public Internal_Id(object value, byte valueSize)
        {
            id = value;
            size = valueSize;
        }

        public abstract bool CompareWithOther(Internal_Id other);
        
        public static bool operator ==(Internal_Id first, Internal_Id second)
        {
            if (Equals(first, null) && Equals(second, null)) return true;
            if (Equals(first, null)) return false;
            if (Equals(second, null)) return false;

            var maximalValue = first.size > second.size ? first : second;
            var otherValue = first.size > second.size ? second : first;

            return maximalValue.CompareWithOther(otherValue);
        }

        public static bool operator !=(Internal_Id first, Internal_Id second)
        {
            return !(first == second);
        }
    }
    
    private class Internal_Id8 : Internal_Id
    {
        public Internal_Id8(byte value) : base(value, 1) { }

        public override bool CompareWithOther(Internal_Id other)
        {
            return (byte)id == (byte)other.id;
        } 
    }
    
    private class Internal_Id16 : Internal_Id
    {
        public Internal_Id16(short value) : base(value, 2) { }

        public override bool CompareWithOther(Internal_Id other)
        {
            if (other.id is byte byteValue)
                return (short)id == byteValue;
            
            return (short)id == (short)other.id;
        }
    }
    
    private class Internal_Id32 : Internal_Id
    {
        public Internal_Id32(int value) : base(value, 4) { }

        public override bool CompareWithOther(Internal_Id other)
        {
            if (other.id is byte byteValue)
                return (int)id == byteValue;
            if (other.id is short shortValue)
                return (int)id == shortValue;
            
            return (int)id == (int)other.id;
        }
    }

    private class Internal_Id64 : Internal_Id
    {
        public Internal_Id64(long value) : base(value, 8) { }

        public override bool CompareWithOther(Internal_Id other)
        {
            if (other.id is byte byteValue)
                return (long)id == byteValue;
            if (other.id is short shortValue)
                return (long)id == shortValue;
            if (other.id is int intValue)
                return (long)id == intValue;
            
            return (long)id == (long)other.id;
        }
    }
    
    private class Internal_Id128 : Internal_Id
    {
        public Internal_Id128(decimal value) : base(value, 16) { }

        public override bool CompareWithOther(Internal_Id other)
        {
            if (other.id is byte byteValue)
                return (decimal)id == byteValue;
            if (other.id is short shortValue)
                return (decimal)id == shortValue;
            if (other.id is int intValue)
                return (decimal)id == intValue;
            if (other.id is long longValue)
                return (decimal)id == longValue;
            
            return (long)id == (decimal)other.id;
        }
    }
}
