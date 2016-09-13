namespace Vurdalakov
{
    using System;
    using System.Globalization;
    using System.Text;

    public static class StringExtensions
    {
        public static Boolean ContainsWord(this String text, String word)
        {
            return !String.IsNullOrEmpty(text) && (text.Equals(word) || text.StartsWith(word + " ") || text.Contains(" " + word + " ") || text.EndsWith(" " + word));
        }

        public static String ToUpperFirst(this String text, CultureInfo cultureInfo = null)
        {
            if (null == cultureInfo)
            {
                cultureInfo = CultureInfo.CurrentUICulture;
            }

            return String.IsNullOrEmpty(text) ? text : Char.ToUpper(text[0], cultureInfo) + text.Substring(1);
        }

        public static String XmlEscape(this String text)
        {
            return String.IsNullOrEmpty(text) ? text : text.Replace("\"", "&quot;").Replace("'", "&apos;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
        }

        public static String XmlUnescape(this String text)
        {
            return String.IsNullOrEmpty(text) ? text : text.Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");
        }

        public static String Base64Encode(this String text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static String Base64Decode(this String text)
        {
            var bytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string Reverse(this String text)
        {
            var charArray = text.ToCharArray();
            Array.Reverse(charArray);
            return new String(charArray);
        }
    }
}
