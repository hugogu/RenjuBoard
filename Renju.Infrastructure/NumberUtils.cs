using System;

namespace Renju.Infrastructure
{
    public static class NumberUtils
    {
        private static readonly Random _random = new Random();

        public static bool InRang<T>(this T number, T limit, T anotherLimit)
            where T : IComparable<T>
        {
            return number.CompareTo(limit) <= 0 && number.CompareTo(anotherLimit) >= 0 ||
                   number.CompareTo(limit) >= 0 && number.CompareTo(anotherLimit) <= 0;
        }

        public static bool WithInRang<T>(this T number, T limit, T anotherLimit)
            where T : IComparable<T>
        {
            return number.CompareTo(limit) < 0 && number.CompareTo(anotherLimit) > 0 ||
                   number.CompareTo(limit) > 0 && number.CompareTo(anotherLimit) < 0;
        }

        public static int NewRandom()
        {
            return _random.Next();
        }
    }
}
