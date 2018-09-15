using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Dota_Geek.DataTypes;
using Newtonsoft.Json;

namespace Dota_Geek.Modules
{
    public class Subscribe : ModuleBase<SocketCommandContext>
    {
        [Command("i am", RunMode = RunMode.Async)]
        [Alias("iam")]
        public async Task SubscribeTask(string account)
        {
            var url = $"https://steamid.venner.io/raw.php?input={account}";
            using (var client = new WebClient())
            {
                var json = client.DownloadString(url);
                var obj = JsonConvert.DeserializeObject<SteamConvertData>(json);

                string reply;
                if (LinkedAccounts.UserDictionary.TryAdd(Context.User.Id, obj.Uid))
                {
                    reply = $"Now I know you as [{obj.Name}](http://steamcommunity.com/profiles/{obj.Steamid64}) 🙂";
                    LinkedAccounts.Save();
                }
                else
                {
                    url = $"https://steamid.venner.io/raw.php?input={LinkedAccounts.UserDictionary[Context.User.Id]}";
                    json = client.DownloadString(url);
                    obj = JsonConvert.DeserializeObject<SteamConvertData>(json);
                    reply =
                        $"But But But I already know you as [{obj.Name}](http://steamcommunity.com/profiles/{obj.Steamid64}) 😕";
                }

                var embed = new EmbedBuilder {Description = reply};
                await ReplyAsync(string.Empty, false, embed.Build());
            }
        }
    }
}