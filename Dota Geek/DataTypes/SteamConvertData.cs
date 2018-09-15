using System;
using Newtonsoft.Json;

namespace Dota_Geek.DataTypes
{
    public class SteamConvertData
    {
        [JsonProperty("interpreted")] public string Interpreted { get; set; }

        [JsonProperty("avatar")] public Uri Avatar { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("steamid")] public string Steamid { get; set; }

        [JsonProperty("steamid64")] public long Steamid64 { get; set; }

        [JsonProperty("uid")] public long Uid { get; set; }
    }
}