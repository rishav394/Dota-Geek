using Newtonsoft.Json;

namespace Dota_Geek.DataTypes.OpenDota
{
    public class ProTeamPlayers
    {
        [JsonProperty("account_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? AccountId { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("games_played", NullValueHandling = NullValueHandling.Ignore)]
        public long? GamesPlayed { get; set; }

        [JsonProperty("wins", NullValueHandling = NullValueHandling.Ignore)]
        public long? Wins { get; set; }

        [JsonProperty("is_current_team_member")]
        public bool? IsCurrentTeamMember { get; set; }
    }
}