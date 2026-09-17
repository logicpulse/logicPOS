using System.Globalization;

namespace LogicPOS.Utility
{
    public static class TextSearch
    {
        private static readonly CompareInfo Compare = CultureInfo.CurrentCulture.CompareInfo;

        private const CompareOptions MatchOptions =
            CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

        public static bool Contains(string text, string key)
        {
            text = text?.Trim() ?? string.Empty;
            key = key?.Trim() ?? string.Empty;

            if (key.Length == 0)
            {
                return true;
            }

            if (text.Length == 0)
            {
                return false;
            }

            return Compare.IndexOf(text, key, MatchOptions) >= 0;
        }
    }
}
