using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Dota_Geek
{
    internal class Program
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;

        static void Main()
        {
            new Program().InitializeAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeAsync()
        {
            if (string.IsNullOrEmpty(Config.Bot.Token))
            {
                Console.WriteLine("No token detected");
                Console.ReadKey();
                return;
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            _client.Log += ClientLog;
            _client.Ready += ClientReady;

            await _client.LoginAsync(TokenType.Bot, Config.Bot.Token);
            await _client.StartAsync();
            await _client.SetGameAsync("Dota 2", null, ActivityType.Watching);

            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
        }

        private Task ClientReady()
        {
            throw new NotImplementedException();
        }

        private Task ClientLog(LogMessage arg)
        {
            throw new NotImplementedException();
        }
    }
}
