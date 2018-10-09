using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using Dota_Geek.DataTypes;

namespace Dota_Geek.Modules
{
    public class HeroSpecific : ModuleBase<SocketCommandContext>
    {
        [Command("talent")]
        [Alias("talents")]
        public async Task HeroTalentTask([Summary("Hero name first few letters")] [Remainder] string hero)
        {
            var data = File.ReadAllText("DataTypes\\Talents.txt");
            
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;

            hero = textInfo.ToTitleCase(hero);
            var lines = data.Split("\n").ToList();
            var pattern = $"dline\" id=\"{hero.Split(" ").First()}";
            var regex = new Regex(pattern);
            var ind = lines.IndexOf(lines.First(x => regex.IsMatch(x)));
            var newList = lines.Skip(ind).Where(x => x.Contains(@"<td width=""280"">")).Take(8).ToList();
            var i = 5;
            var temp = string.Empty;
            var j = 0;
            var my = $"```";
            foreach (var s in newList)
            {
                var a = s.Replace(@"<td width=""280"">", "");
                a = Regex.Replace(a, @".+<b>(.+)</b>.+", "$1");
                a = a.Replace("\n", "");
                a = a.Trim();
                if (j % 2 == 0)
                {
                    temp = a;
                }
                else
                {
                    my +=
                        $"{(i-- * 5).ToString().PadLeft(2)} => {temp.Truncate(25).PadLeft(25)}" +
                        $" | {a.Truncate(25).PadRight(25)}\n";
                }
                
                j++;
            }

            my += "```";
            Console.WriteLine(my);
            await ReplyAsync(my);
        }
    }
}
