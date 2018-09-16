namespace Dota_Geek.DataTypes.OpenDota
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ProTeamMatch
    {
        [JsonProperty("match_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? MatchId { get; set; }

        [JsonProperty("radiant_win", NullValueHandling = NullValueHandling.Ignore)]
        public bool RadiantWin { get; set; }

        [JsonProperty("radiant", NullValueHandling = NullValueHandling.Ignore)]
        public bool Radiant { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public long? Duration { get; set; }

        [JsonProperty("start_time", NullValueHandling = NullValueHandling.Ignore)]
        public long StartTime { get; set; }

        [JsonProperty("leagueid", NullValueHandling = NullValueHandling.Ignore)]
        public long? Leagueid { get; set; }

        [JsonProperty("league_name", NullValueHandling = NullValueHandling.Ignore)]
        public string LeagueName { get; set; }

        [JsonProperty("cluster", NullValueHandling = NullValueHandling.Ignore)]
        public long? Cluster { get; set; }

        [JsonProperty("opposing_team_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? OpposingTeamId { get; set; }

        [JsonProperty("opposing_team_name", NullValueHandling = NullValueHandling.Ignore)]
        public string OpposingTeamName { get; set; }

        [JsonProperty("opposing_team_logo")]
        public Uri OpposingTeamLogo { get; set; }
    }
}
