namespace Dota_Geek.DataTypes
{
    internal static class NameTruncate
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 2) + "..";
        }

        public static string Times(this int times, char value)
        {
            var my = string.Empty;
            for (var i = 0; i < times; i++)
            {
                my += value;
            }

            return my;
        }
    }
}