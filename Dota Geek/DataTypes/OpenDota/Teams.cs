using System;
using Newtonsoft.Json;

namespace Dota_Geek.DataTypes.OpenDota
{
    public class Teams
    {
        [JsonProperty("team_id")] public long TeamId { get; set; }

        [JsonProperty("rating")] public double Rating { get; set; }

        [JsonProperty("wins")] public long Wins { get; set; }

        [JsonProperty("losses")] public long Losses { get; set; }

        [JsonProperty("last_match_time")] public long LastMatchTime { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("tag")] public string Tag { get; set; }

        [JsonProperty("logo_url")] public Uri LogoUrl { get; set; }
    }
}