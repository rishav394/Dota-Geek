namespace Dota_Geek.DataTypes.OpenDota
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class PlayerProfile
    {
        [JsonProperty("tracked_until")]
        public long TrackedUntil { get; set; }

        [JsonProperty("profile")]
        public Profile Profile { get; set; }

        [JsonProperty("solo_competitive_rank")]
        public dynamic SoloCompetitiveRank { get; set; }

        [JsonProperty("competitive_rank")]
        public dynamic CompetitiveRank { get; set; }

        [JsonProperty("rank_tier")]
        public long RankTier { get; set; }

        [JsonProperty("leaderboard_rank")]
        public dynamic LeaderboardRank { get; set; }

        [JsonProperty("mmr_estimate")]
        public MmrEstimate MmrEstimate { get; set; }
    }

    public partial class MmrEstimate
    {
        [JsonProperty("estimate")]
        public long Estimate { get; set; }
    }

    public partial class Profile
    {
        [JsonProperty("account_id")]
        public long AccountId { get; set; }

        [JsonProperty("personaname")]
        public string Personaname { get; set; }

        [JsonProperty("name")]
        public dynamic Name { get; set; }

        [JsonProperty("cheese")]
        public long Cheese { get; set; }

        [JsonProperty("steamid")]
        public string Steamid { get; set; }

        [JsonProperty("avatar")]
        public Uri Avatar { get; set; }

        [JsonProperty("avatarmedium")]
        public Uri Avatarmedium { get; set; }

        [JsonProperty("avatarfull")]
        public Uri Avatarfull { get; set; }

        [JsonProperty("profileurl")]
        public Uri Profileurl { get; set; }

        [JsonProperty("last_login")]
        public DateTimeOffset LastLogin { get; set; }

        [JsonProperty("loccountrycode")]
        public string Loccountrycode { get; set; }
    }
}
