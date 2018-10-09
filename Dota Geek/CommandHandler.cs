using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Dota_Geek
{
    // Should handle everything related to client and commands ONLY
    internal class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private CommandService _command;
        private IServiceProvider _services;
        private DiscordBotListHandler _discordBotListHandler;

        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task InitializeAsync()
        {
            _command = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            _discordBotListHandler = new DiscordBotListHandler(485759803155546113, Config.Bot.DblToken, _client);

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_command)
                .BuildServiceProvider();

            await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.Ready += Ready;
            _client.MessageReceived += HandleCommandAsync;
            _client.JoinedGuild += JoinAsync;
            _client.LeftGuild += LeftAsync;
            _client.Log += Log;
        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        private async Task Ready()
        {
            await _discordBotListHandler.UpdateAsync();
        }

        private async Task LeftAsync(SocketGuild arg)
        {
            await _discordBotListHandler.UpdateAsync();
            var channel = _client.GetGuild(480857253092524032).GetChannel(491281793039859723) as ITextChannel;
            if (channel is null) return;
            await channel.SendMessageAsync($"I just left {arg.Name} :slight_frown: ");
        }

        private async Task JoinAsync(SocketGuild arg)
        {
            await _discordBotListHandler.UpdateAsync();
            var channel = _client.GetGuild(480857253092524032).GetChannel(491281793039859723) as ITextChannel;
            if (channel is null) return;
            await channel.SendMessageAsync($"I just joined {arg.Name} :tada:");
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            var context = new SocketCommandContext(_client, msg);
            var argPos = 0;

            if (!Config.Bot.PrefixDictionary.ContainsKey(context.Guild.Id))
            {
                Config.Bot.PrefixDictionary.Add(context.Guild.Id, "$");
                Config.Save();
            }

            if (msg.HasStringPrefix(Config.Bot.PrefixDictionary[context.Guild.Id], ref argPos) ||
                msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                try
                {
                    var typing = context.Channel.EnterTypingState();
                    var result = await _command.ExecuteAsync(context, argPos, _services);
                    typing.Dispose();
                    if (!result.IsSuccess)
                    {
                        Console.WriteLine(result.ErrorReason + $" at {context.Guild.Name}");
                        switch (result.Error)
                        {
                            case CommandError.UnknownCommand:
                            {
                                var guildEmote = Emote.Parse("<:unknowscmd:461157571701506049>");
                                await msg.AddReactionAsync(guildEmote);
                                break;
                            }
                            case CommandError.BadArgCount:
                            {
                                await context.Channel.SendMessageAsync($"You are suppose to pass in a parameter with" +
                                                                       $" this command. type `help` for help");
                                break;
                            }
                            case CommandError.UnmetPrecondition:
                            {
                                await context.Channel.SendMessageAsync(
                                    $"You don have enough permissions to use this command" +
                                    result.ErrorReason);
                                break;
                            }
                            default:
                            {
                                await context.Channel.SendMessageAsync(result.Error.ToString());
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
            }
        }
    }
}