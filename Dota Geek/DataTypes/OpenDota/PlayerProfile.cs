using Newtonsoft.Json;

namespace Dota_Geek.DataTypes.OpenDota
{
    public class PlayerProfile
    {
        [JsonProperty("tracked_until", NullValueHandling = NullValueHandling.Ignore)]
        public string TrackedUntil { get; set; }

        [JsonProperty("solo_competitive_rank", NullValueHandling = NullValueHandling.Ignore)]
        public string SoloCompetitiveRank { get; set; }

        [JsonProperty("competitive_rank", NullValueHandling = NullValueHandling.Ignore)]
        public string CompetitiveRank { get; set; }

        [JsonProperty("rank_tier", NullValueHandling = NullValueHandling.Ignore)]
        public long? RankTier { get; set; }

        [JsonProperty("leaderboard_rank", NullValueHandling = NullValueHandling.Ignore)]
        public long? LeaderboardRank { get; set; }

        [JsonProperty("mmr_estimate", NullValueHandling = NullValueHandling.Ignore)]
        public MmrEstimate MmrEstimate { get; set; }

        [JsonProperty("profile", NullValueHandling = NullValueHandling.Ignore)]
        public Profile Profile { get; set; }
    }

    public class MmrEstimate
    {
        [JsonProperty("estimate", NullValueHandling = NullValueHandling.Ignore)]
        public long? Estimate { get; set; }

        [JsonProperty("stdDev", NullValueHandling = NullValueHandling.Ignore)]
        public long? StdDev { get; set; }

        [JsonProperty("n", NullValueHandling = NullValueHandling.Ignore)]
        public long? N { get; set; }
    }

    public class Profile
    {
        [JsonProperty("account_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccountId { get; set; }

        [JsonProperty("personaname", NullValueHandling = NullValueHandling.Ignore)]
        public string Personaname { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("cheese", NullValueHandling = NullValueHandling.Ignore)]
        public long? Cheese { get; set; }

        [JsonProperty("steamid", NullValueHandling = NullValueHandling.Ignore)]
        public string Steamid { get; set; }

        [JsonProperty("avatar", NullValueHandling = NullValueHandling.Ignore)]
        public string Avatar { get; set; }

        [JsonProperty("avatarmedium", NullValueHandling = NullValueHandling.Ignore)]
        public string Avatarmedium { get; set; }

        [JsonProperty("avatarfull", NullValueHandling = NullValueHandling.Ignore)]
        public string Avatarfull { get; set; }

        [JsonProperty("profileurl", NullValueHandling = NullValueHandling.Ignore)]
        public string Profileurl { get; set; }

        [JsonProperty("last_login", NullValueHandling = NullValueHandling.Ignore)]
        public string LastLogin { get; set; }

        [JsonProperty("loccountrycode", NullValueHandling = NullValueHandling.Ignore)]
        public string Loccountrycode { get; set; }
    }
}