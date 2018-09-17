using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Dota_Geek.Modules;

namespace Dota_Geek
{
    internal class Program
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;
        private DiscordBotListHandler _discordBotListHandler;


        private static void Main()
        {
            Global.Interval = 60 * 60 * 1000;
            var timer = new System.Timers.Timer(Global.Interval) {Enabled = true};
            timer.Elapsed += Timer_Elapsed;
            
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        private static async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var pair in TrackedAccounts.TrackDictionary)
            {
                var final = Dota.LastMatch($"[U:1:{pair.Key}]", out var lastHour);
                if (lastHour)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("POST " + final.Split('\n')[0]);
                    Console.ResetColor();

                    foreach (var data in pair.Value)
                    {
                        var myChannel = Global.Client.GetGuild(data.GuildId).GetChannel(data.ChannelId) as ITextChannel;
                        if (myChannel is null)
                        {
                            continue;
                        }

                        await myChannel.SendMessageAsync(final);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("SKIP " + final.Split('\n')[0]);
                    Console.ResetColor();
                }
            }
        }

        private async Task StartAsync()
        {
            if (string.IsNullOrEmpty(Config.Bot.Token)) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
            });

            _client.Ready += Ready;
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, Config.Bot.Token);
            await _client.StartAsync();
            await _client.SetGameAsync("Dota 2", null, ActivityType.Watching);

            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1);
        }

        private async Task Ready()
        {
            Global.Client = _client;
            _discordBotListHandler = new DiscordBotListHandler(485759803155546113, Config.Bot.DblToken);
            await _discordBotListHandler.UpdateAsync();
        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }
    }
}
