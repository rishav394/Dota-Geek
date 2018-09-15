using System.Net;
using Newtonsoft.Json;

namespace Dota_Geek.DataTypes
{
    public static class SteamIdParser
    {
        public static long Steam32Parse(this string query)
        {
            var url = $"https://steamid.venner.io/raw.php?input={query}";
            using (var client = new WebClient())
            {
                var json = client.DownloadString(url);
                var obj = JsonConvert.DeserializeObject<SteamConvertData>(json);
                return obj.Uid;
            }
        }

        public static long Steam64Parse(this string query)
        {
            var url = $"https://steamid.venner.io/raw.php?input={query}";
            using (var client = new WebClient())
            {
                var json = client.DownloadString(url);
                var obj = JsonConvert.DeserializeObject<SteamConvertData>(json);
                return obj.Steamid64;
            }
        }
    }
}