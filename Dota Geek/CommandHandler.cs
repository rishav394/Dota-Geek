using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Dota_Geek
{
    internal class CommandHandler
    {
        /// <summary> The <see cref="DiscordSocketClient"/> </summary>
        private DiscordSocketClient _client;

        /// <summary> The <see cref="CommandService"/> </summary>
        private CommandService _service;

        /// <summary> The <see cref="IServiceProvider"/> </summary>
        private IServiceProvider _services;

        /// <summary> Initialize the bot </summary>
        /// <param name="currentClient"> The used <see cref="DiscordSocketClient"/> </param>
        /// <returns> bot initialization </returns>
        public async Task InitializeAsync(DiscordSocketClient currentClient)
        {
            _client = currentClient;

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .BuildServiceProvider();

            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);

            _client.JoinedGuild += JoinEvent;
            _client.LeftGuild += LeftGuild;
            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task LeftGuild(SocketGuild arg)
        {
            var channel = _client.GetGuild(480857253092524032).GetChannel(490821492079329290) as ITextChannel;
            if (channel is null)
            {
                return;
            }

            await channel.SendMessageAsync("I left " + arg.Name + "😫");
        }

        private async Task JoinEvent(SocketGuild arg)
        {
            var channel = _client.GetGuild(480857253092524032).GetChannel(490821492079329290) as ITextChannel;
            if (channel is null)
            {
                return;
            }

            await channel.SendMessageAsync("I joined " + arg.Name + "🎉");
        }

        private async Task HandleMessageAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);

            if (context.User.IsBot || context.IsPrivate) return;

            var currentPrefix = "$";

            if (Config.Bot.PrefixDictionary.ContainsKey(context.Guild.Id))
            {
                currentPrefix = Config.Bot.PrefixDictionary[context.Guild.Id];
            }
            else
            {
                Config.Bot.PrefixDictionary.Add(context.Guild.Id, currentPrefix);
                Config.Save();
            }

            var argPos = 0;

            if (msg.HasStringPrefix(currentPrefix, ref argPos)
                || msg.HasStringPrefix(currentPrefix.ToUpper(), ref argPos)
                || msg.HasStringPrefix(currentPrefix.ToLowerInvariant(), ref argPos)
                || msg.HasStringPrefix(currentPrefix.First().ToString().ToUpper() + currentPrefix.Substring(1),
                    ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                int targetIndex;
                if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    targetIndex = context.Message.Content.IndexOf('>') + 1;
                }
                else
                {
                    targetIndex = Config.Bot.PrefixDictionary[context.Guild.Id].Length;
                }

                var typing = context.Channel.EnterTypingState();
                var result = await _service.ExecuteAsync(
                    context,
                    context.Message.Content.Substring(targetIndex).Trim(),
                    _services);
                typing.Dispose();

                if (!result.IsSuccess)
                {
                    if (result.Error != CommandError.UnknownCommand)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(
                            $" Error: {result.ErrorReason} at {context.Guild.Name} : {context.Channel.Name}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

            }
        }
    }
}