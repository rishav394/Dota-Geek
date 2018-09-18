using System.Threading.Tasks;
using DiscordBotsList.Api;

namespace Dota_Geek
{
    internal class DiscordBotListHandler
    {
        public DiscordBotListHandler(ulong botDiscordId, string botToken)
        {
            Global.DiscordBotListHandler = new AuthDiscordBotListApi(botDiscordId, botToken);
        }

        public async Task UpdateAsync()
        {
            await Global.MeBot.Result.UpdateStatsAsync(Global.Client.Guilds.Count);
        }
    }
}