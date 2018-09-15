using System.Threading.Tasks;
using Discord.Commands;

namespace Dota_Geek.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingTask()
        {
            await ReplyAsync("pong");
        }
    }
}