using Newtonsoft.Json;

namespace Dota_Geek.DataTypes
{
    public class Heroes
    {
        [JsonProperty("id")] public long Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("localized_name")] public string LocalizedName { get; set; }
    }
}