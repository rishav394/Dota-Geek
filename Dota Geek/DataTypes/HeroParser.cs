using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Dota_Geek.DataTypes
{
    internal static class HeroParser
    {
        private static string HeroName(this int heroId)
        {
            var json = File.ReadAllText("DataTypes\\Heroes.json");
            var obj = JsonConvert.DeserializeObject<List<Heroes>>(json);
            var hero = obj.First(x => x.Id == heroId);
            return hero.LocalizedName;
        }

        public static string HeroName(this long heroId)
        {
            return HeroName((int) heroId);
        }
    }
}