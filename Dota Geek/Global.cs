using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBotsList.Api;

namespace Dota_Geek
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }

        public static int Interval { get; set; }

        public static Task<IDblSelfBot> MeBot => DiscordBotListHandler.GetMeAsync();

        public static AuthDiscordBotListApi DiscordBotListHandler { get; set; }
    }
}