using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Dota_Geek.Modules
{
    [RequireOwner]
    public class OwnerCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Give database")]
        [Summary("Very bad command. Dont ever use.")]
        public async Task GiveDatabaseTask()
        {
            var msg = await ReplyAsync("<a:loader:461159122575032331> Loading Database");
            ZipFile.CreateFromDirectory(@"Resources", @"Resources.zip", CompressionLevel.Optimal, true);
            await Context.Channel.SendFileAsync("Resources.zip");
            File.Delete("Resources.zip");
            await msg.DeleteAsync();
        }

        [Command("Make pro")]
        [Summary("Make someone pro user. Requires mention.")]
        public async Task ProTask(SocketUser socketUser)
        {
            var result = socketUser.MakePro();
            if (result)
                await ReplyAsync($"Congratzzz {socketUser.Mention}, you are now a pro member :tada:");
            else
                await ReplyAsync($"{socketUser.Username} is already a pro Member <a:thinkez:462208367121661955>");
        }

        [Command("Remove pro")]
        [Summary("Revoke pro membership from someone. Requires mention.")]
        public async Task RemoveProTask(SocketUser socketUser)
        {
            var result = socketUser.RemovePro();
            if (result)
                await ReplyAsync($"{socketUser.Mention}, you are no longer a pro member <:rip:462208313480708096>");
            else
                await ReplyAsync($"{socketUser.Username} was never pro <:kyloren:461284603873853460>");
        }
    }
}