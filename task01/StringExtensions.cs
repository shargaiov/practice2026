using System;
namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            string cleaned = "";
            string reversed = "";

            foreach (char c in input.ToLower())
            {
                if (!char.IsPunctuation(c) && !char.IsWhiteSpace(c))
                {
                    cleaned = cleaned + c;
                    reversed = c + reversed;
                }
            }

            return cleaned.Length > 0 && cleaned == reversed;
        }
    }
}


















