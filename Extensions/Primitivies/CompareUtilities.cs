namespace Rietmon.Extensions
{
    public static class CompareUtilities
    {
        public static bool? PreCompare(object first, object second)
        {
            var isFirstNull = Equals(first, null);
            var isSecondNull = Equals(second, null);

            if (isFirstNull && isSecondNull)
                return true;

            if (isFirstNull != isSecondNull)
                return false;

            return null;
        }
    }
}