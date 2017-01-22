namespace HIT.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsAny(this string text, char[] characters)
        {
            var indexNotFound = -1;
            var containsAny = text.IndexOfAny(characters) != indexNotFound;

            return containsAny;
        }
    }
}
