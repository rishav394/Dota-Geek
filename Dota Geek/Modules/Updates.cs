using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Dota_Geek.DataTypes;

namespace Dota_Geek.Modules
{
    [RequireUserPermission(GuildPermission.Administrator)]
    public class Updates : ModuleBase<SocketCommandContext>
    {
        [Command("patches")]
        [Summary("Sends you Dota 2 patches in this channel")]
        public async Task PatchesTask()
        {
            var data = new SendData
            {
                GuildId = Context.Guild.Id,
                ChannelId = Context.Channel.Id
            };

            if (UpdateReceivers.Patches.Contains(data))
            {
                await ReplyAsync(
                    $"I am already sending Dota 2 updates in {Context.Guild.GetTextChannel(data.ChannelId)?.Name}" +
                    "\nTo stop use `no patches`");
                return;
            }

            UpdateReceivers.Append(data);
            await ReplyAsync("Great, I will send Dota 2 patches in this channel :tada:");
        }

        [Command("no patches")]
        [Summary("No more patch notes for you noob")]
        [Alias("nopatches")]
        public async Task NoPatchesTask()
        {
            UpdateReceivers.Remove(Context.Guild.Id);
            await ReplyAsync("I'm sad to see you go 🤕");
        }
    }
}