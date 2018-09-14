using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Dota_Geek.DataTypes;
using Newtonsoft.Json;

namespace Dota_Geek
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
    }
}
