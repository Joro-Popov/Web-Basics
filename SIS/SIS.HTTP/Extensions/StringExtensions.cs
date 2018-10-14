namespace SIS.HTTP.Extensions
{
    using System.Linq;

    public static class StringExtensions
    {
        public static string Capitalize(this string value)
        {
            var capitalizedValue = value.First().ToString().ToUpper() + value.Substring(1).ToLower();

            return capitalizedValue;
        }
    }
}