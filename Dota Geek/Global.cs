using Discord.WebSocket;

namespace Dota_Geek
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        public static int Interval { get; set; }
    }
}