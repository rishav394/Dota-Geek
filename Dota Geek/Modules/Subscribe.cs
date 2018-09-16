using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dota_Geek.DataTypes;
using Newtonsoft.Json;

namespace Dota_Geek.Modules
{
    [Name("Define yourself")]
    public class Subscribe : ModuleBase<SocketCommandContext>
    {
        [Command("Who is", RunMode = RunMode.Async)]
        [Summary("Ill let you know who someone is if I know him")]
        public async Task WhoIsTask(SocketGuildUser userMention)
        {
            if (LinkedAccounts.UserDictionary.ContainsKey(userMention.Id))
            {
                using (var client = new WebClient())
                {
                    var url = $"https://steamid.venner.io/raw.php?input=[U:1:{LinkedAccounts.UserDictionary[Context.User.Id]}]";
                    var json = client.DownloadString(url);
                    var obj = JsonConvert.DeserializeObject<SteamConvertData>(json);
                    var reply =
                        $"I know {userMention.Username} as [{obj.Name}](http://steamcommunity.com/profiles/{obj.Steamid64}) 🙂";
                    await ReplyAsync(string.Empty, false, new EmbedBuilder
                    {
                        Description = reply
                    }.Build());
                }
            }
            else
            {
                await ReplyAsync(
                    $"Idk who {userMention.Username} is but i'll be glad to meet {userMention.Nickname}" +
                    $" at `{Config.Bot.PrefixDictionary[Context.Guild.Id]}I am [steam ID]`");
            }
        }

        [Command("Who am I", RunMode = RunMode.Async)]
        [Alias("Who am i?")]
        [Summary("That's really a dumb thing for a human to ask a robot who he is")]
        public async Task WhoTask()
        {
            if (LinkedAccounts.UserDictionary.ContainsKey(Context.User.Id))
            {
                using (var client = new WebClient())
                {
                    var url = $"https://steamid.venner.io/raw.php?input=[U:1:{LinkedAccounts.UserDictionary[Context.User.Id]}]";
                    var json = client.DownloadString(url);
                    var obj = JsonConvert.DeserializeObject<SteamConvertData>(json);
                    var reply =
                        $"Nice to meet you [{obj.Name}](http://steamcommunity.com/profiles/{obj.Steamid64}) 🙂";
                    await ReplyAsync(string.Empty, false, new EmbedBuilder
                    {
                        Description = reply
                    }.Build());
                }
            }
            else
            {
                await ReplyAsync("Hmm looks like we have never met before. " +
                                 "\n*but don't you worry Ive arranged us a secret meeting at* `I am [you account]` 😉");
            }
        }

        [Command("I am", RunMode = RunMode.Async)]
        [Summary
            ("Gives your sad life a meaning. After you set this up, commands like `profile` and others wont require a parameter :)")]
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
                    url = $"https://steamid.venner.io/raw.php?input=[U:1:{LinkedAccounts.UserDictionary[Context.User.Id]}]";
                    json = client.DownloadString(url);
                    obj = JsonConvert.DeserializeObject<SteamConvertData>(json);
                    reply =
                        $"But But But I already know you as [{obj.Name}](http://steamcommunity.com/profiles/{obj.Steamid64}) 😕";
                }

                var embed = new EmbedBuilder {Description = reply};
                await ReplyAsync(string.Empty, false, embed.Build());
            }
        }

        [Command("I am not", RunMode = RunMode.Async)]
        [Summary(
            "I will forget you and you will be cursed to give a parameter every-time you use an accountID demanding command")]
        public async Task UnSubscribeTask(string accountId)
        {
            if (LinkedAccounts.UserDictionary.ContainsKey(Context.User.Id))
            {
                if (LinkedAccounts.UserDictionary[Context.User.Id] == accountId.Steam32Parse())
                {
                    LinkedAccounts.UserDictionary.Remove(Context.User.Id);
                    LinkedAccounts.Save();
                    await ReplyAsync("Okay you are no longer " + accountId +
                                     "\nI curse you to give a parameter every-time you use an `accountID` demanding command. RIP!");
                }
                else
                {
                    await ReplyAsync("You were never " + accountId);
                }
            }
            else
            {
                await ReplyAsync(
                    "Uhh I don't know who you are 😑" +
                    "\n*I've arranged us a secret meeting at* `I am [you account]` 😉");
            }
        }
    }
}