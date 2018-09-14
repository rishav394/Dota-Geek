namespace Dota_Geek.DataTypes
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class SteamConvertData
    {
        [JsonProperty("interpreted")]
        public string Interpreted { get; set; }

        [JsonProperty("avatar")]
        public Uri Avatar { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("steamid")]
        public string Steamid { get; set; }

        [JsonProperty("steamid64")]
        public string Steamid64 { get; set; }

        [JsonProperty("uid")]
        public long Uid { get; set; }
    }
}
