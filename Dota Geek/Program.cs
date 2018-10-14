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
        private SuperHandler _handler;

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
            try
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
                        if (!(_client.GetGuild(data.GuildId)?.GetChannel(data.ChannelId) is ITextChannel myChannel)) continue;

                        await myChannel.SendMessageAsync(final);
                    }
                }
            }
            catch (Exception g)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine(g);
                Console.ResetColor();
            }
        }

        private async Task StartAsync()
        {
            if (string.IsNullOrEmpty(Config.Bot.Token)) return;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            _handler = new SuperHandler(_client);
            await _handler.InitializeAsync();
            
            await _client.LoginAsync(TokenType.Bot, Config.Bot.Token);
            await _client.StartAsync();
            await _client.SetGameAsync("Dota 2", null, ActivityType.Watching);
            await ConsoleRead();
            await Task.Delay(-1);
        }

        private async Task ConsoleRead()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                if (input.ToLower() == "announce")
                {
                    Console.WriteLine("What do you want to send?");
                    var what = Console.ReadLine();
                    foreach (var clientGuild in _client.Guilds)
                    {
                        try
                        {
                            await clientGuild.DefaultChannel.SendMessageAsync(what);
                            Console.WriteLine(
                                $"Sent to {clientGuild.Name}@{clientGuild.DefaultChannel.Name} successfully.");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Could not send to {clientGuild.Name}.");
                        }
                    }
                }
                
                await Task.CompletedTask;
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}