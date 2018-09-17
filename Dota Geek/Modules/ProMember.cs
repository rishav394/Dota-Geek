using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Dota_Geek.Modules
{
    [RequireOwner]
    public class ProMember : ModuleBase<SocketCommandContext>
    {
        [Command("Make pro")]
        public async Task ProTask(SocketUser socketUser)
        {
            var result = socketUser.MakePro();
            if (result)
            {
                await ReplyAsync($"Congratzzz {socketUser.Mention}, you are now a pro member :tada:");
            }
            else
            {
                await ReplyAsync($"{socketUser.Username} is already a pro Member <a:thinkez:462208367121661955>");
            }
        }

        [Command("Remove pro")]
        public async Task RemoveProTask(SocketUser socketUser)
        {
            var result = socketUser.RemovePro();
            if (result)
            {
                await ReplyAsync($"{socketUser.Mention}, you are no longer a pro member <:rip:462208313480708096>");
            }
            else
            {
                await ReplyAsync($"{socketUser.Username} was never pro <:kyloren:461284603873853460>");
            }
        }
    }
}
