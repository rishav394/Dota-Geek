using Newtonsoft.Json;

namespace Dota_Geek.DataTypes.OpenDota
{
    public class HeroPlayData
    {
        [JsonProperty("hero_id")] public long HeroId { get; set; }

        [JsonProperty("last_played")] public long LastPlayed { get; set; }

        [JsonProperty("games")] public long Games { get; set; }

        [JsonProperty("win")] public long Win { get; set; }

        [JsonProperty("with_games")] public long WithGames { get; set; }

        [JsonProperty("with_win")] public long WithWin { get; set; }

        [JsonProperty("against_games")] public long AgainstGames { get; set; }

        [JsonProperty("against_win")] public long AgainstWin { get; set; }
    }
}