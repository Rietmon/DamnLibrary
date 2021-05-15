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

    public static Id CreateFrom8Bytes(byte value)
    {
        return new Id
        {
            id8 = new Internal_Id8(value)
        };
    }
    
    public static Id CreateFrom16Bytes(short value)
    {
        return new Id
        {
            id16 = new Internal_Id16(value)
        };
    }
    
    public static Id CreateFrom32Bytes(int value)
    {
        return new Id
        {
            id32 = new Internal_Id32(value)
        };
    }
    
    public static Id CreateFrom64Bytes(long value)
    {
        return new Id
        {
            id64 = new Internal_Id64(value)
        };
    }
    
    public static Id CreateFrom128Bytes(decimal value)
    {
        return new Id
        {
            id128 = new Internal_Id128(value)
        };
    }

    public static bool operator ==(Id first, Id second)
    {
        return first != null && second != null && first.UsedId == second.UsedId;
    }

    public static bool operator !=(Id first, Id second)
    {
        return !(first == second);
    }

    private abstract class Internal_Id
    {
        public readonly object id;

        public Internal_Id(object value) => id = value;

        protected bool Equals(Internal_Id other)
        {
            return EqualityComparer<object>.Default.Equals(id, other.id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Internal_Id)obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<object>.Default.GetHashCode(id);
        }
        
        public static bool operator ==(Internal_Id first, Internal_Id second)
        {
            return first != null && second != null && first.id.Equals(second.id);
        }

        public static bool operator !=(Internal_Id first, Internal_Id second)
        {
            return !(first == second);
        }
    }
    
    private class Internal_Id8 : Internal_Id
    {
        public Internal_Id8(byte value) : base(value) { }
    }
    
    private class Internal_Id16 : Internal_Id
    {
        public Internal_Id16(short value) : base(value) { }
    }
    
    private class Internal_Id32 : Internal_Id
    {
        public Internal_Id32(int value) : base(value) { }
    }

    private class Internal_Id64 : Internal_Id
    {
        public Internal_Id64(long value) : base(value) { }
    }
    
    private class Internal_Id128 : Internal_Id
    {
        public Internal_Id128(decimal value) : base(value) { }
    }
}
