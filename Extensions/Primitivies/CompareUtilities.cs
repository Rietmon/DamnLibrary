namespace Rietmon.Extensions
{
    public static class CompareUtilities
    {
        /// <summary>
        /// Pre compare for classes
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>
        /// true - objects are equals,
        /// false - objects are different,
        /// null - can't compare
        /// </returns>
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