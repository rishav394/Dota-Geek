using System;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.WebSocket;
using Dota_Geek.Modules;

namespace Dota_Geek
{
    internal class Program
    {
        private static DiscordSocketClient _client;
        private CommandHandler _handler;


        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            Global.Interval = 60 * 60 * 1000;
            var timer = new Timer(Global.Interval) {Enabled = true};
            timer.Elapsed += Timer_Elapsed;

            new Program().StartAsync().GetAwaiter().GetResult();
        }

        private static async void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(e.ToString());
            Console.ResetColor();

            if (_client.GetGuild(480857253092524032).GetChannel(495978647635623937) is ITextChannel important)
                await important.SendMessageAsync(e.ToString());
        }

        private static async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var pair in TrackedAccounts.TrackDictionary)
            {
                var final = Dota.LastMatch($"[U:1:{pair.Key}]", out var lastHour);
                if (pair.Value is null || !lastHour)
                {
                    continue;
                }
                
                foreach (var data in pair.Value)
                {
                    var myChannel =
                        _client.GetGuild(data.GuildId)?.GetChannel(data.ChannelId) as ITextChannel;
                    if (myChannel is null) continue;

                    await myChannel.SendMessageAsync(final);
                }
            }

        }

        private async Task StartAsync()
        {
            if (string.IsNullOrEmpty(Config.Bot.Token)) return;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            _handler = new CommandHandler(_client); // Add LavaLinkManager
            await _handler.InitializeAsync();
            
            await _client.LoginAsync(TokenType.Bot, Config.Bot.DevToken);
            await _client.StartAsync();
            await _client.SetGameAsync("Dota 2", null, ActivityType.Watching);

            await Task.Delay(-1);
        }
    }
}