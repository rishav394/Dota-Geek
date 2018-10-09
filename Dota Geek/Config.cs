using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Dota_Geek
{
    internal static class Config
    {
        public static BotConfig Bot;

        static Config()
        {
            if (!Directory.Exists("Resources")) Directory.CreateDirectory("Resources");

            if (File.Exists("Resources/Config.json"))
            {
                Bot = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("Resources/Config.json"));
            }
            else
            {
                Bot = new BotConfig
                {
                    Token = null,
                    DevToken = null,
                    PrefixDictionary = new Dictionary<ulong, string>(),
                    DblToken = null
                };
                Save();
            }
        }

        public static void Save()
        {
            var json = JsonConvert.SerializeObject(Bot, Formatting.Indented);
            File.WriteAllText("Resources/Config.json", json);
        }

        public struct BotConfig
        {
            public string Token { get; set; }
            public string DevToken { get; set; }
            public Dictionary<ulong, string> PrefixDictionary { get; set; }
            public string DblToken { get; set; }
        }
    }
}