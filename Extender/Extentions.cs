using System;

namespace Extender
{
    static class Extentions
    {
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        public static T[] Resize<T>(this T[] array, int newSize)
        {
            Array.Resize(ref array, newSize);
            return array;
        }
    }
}
