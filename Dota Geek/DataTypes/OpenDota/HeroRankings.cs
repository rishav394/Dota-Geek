namespace Dota_Geek.DataTypes.OpenDota
{
    using Newtonsoft.Json;

    public partial class HeroRankings
    {
        [JsonProperty("hero_id")]
        public long HeroId { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("percent_rank")]
        public double PercentRank { get; set; }

        [JsonProperty("card")]
        public long Card { get; set; }
    }
}
