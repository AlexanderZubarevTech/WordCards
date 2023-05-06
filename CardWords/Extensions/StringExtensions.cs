namespace CardWords.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string? str) 
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNullOrEmptyOrWhiteSpace(this string? str)
        {
            return str.IsNullOrEmpty() || str.IsNullOrWhiteSpace();
        }
    }
}
