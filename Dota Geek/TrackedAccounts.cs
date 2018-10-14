using System.Collections.Generic;
using System.IO;
using Dota_Geek.DataTypes;
using Newtonsoft.Json;

namespace Dota_Geek
{
    public static class TrackedAccounts
    {
        // <Steam32 : List< GuildID and ChannelID >>
        public static Dictionary<long, List<SendData>> TrackDictionary { get; private set; }

        static TrackedAccounts()
        {
            if (File.Exists("Resources/TrackedAccounts.json"))
            {
                var file = File.ReadAllText("Resources/TrackedAccounts.json");
                TrackDictionary = JsonConvert.DeserializeObject<Dictionary<long, List<SendData>>>(file);
            }
            else
            {
                TrackDictionary = new Dictionary<long, List<SendData>>();
                Save();
            }
        }

        public static void Save()
        {
            var json = JsonConvert.SerializeObject(TrackDictionary, Formatting.Indented);
            File.WriteAllText("Resources/TrackedAccounts.json", json);
        }
    }
}