using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Dota_Geek.Modules
{
    [Name("Admin Commands")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class Admin : ModuleBase<SocketCommandContext>
    {
        [Command("prefix")]
        [Summary("Change or view my prefix")]
        public async Task PrefixTask(string newPrefix = null)
        {
            if (string.IsNullOrEmpty(newPrefix))
            {
                await ReplyAsync($"Hmm it was hard to remember but I think I am knows as" +
                                 $" `{Config.Bot.PrefixDictionary[Context.Guild.Id]}` in this Party.");
            }
            else
            {
                Config.Bot.PrefixDictionary[Context.Guild.Id] = newPrefix;
                Config.Save();
                await ReplyAsync($"Alright Alright in the latest patch people here have to call me `{newPrefix}`");
            }
        }
    }
}
