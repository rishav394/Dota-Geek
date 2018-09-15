using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Dota_Geek
{
    internal class CommandHandler
    {
        private DiscordSocketClient _client;

        private CommandService _service;

        private IServiceProvider _services;

        public async Task InitializeAsync(DiscordSocketClient currentClient)
        {
            _client = currentClient;

            _services = new ServiceCollection()
                .BuildServiceProvider();

            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);

            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task HandleMessageAsync(SocketMessage s)
        {
            var msg = (SocketUserMessage) s;
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
                await context.Channel.TriggerTypingAsync();

                var result = await _service.ExecuteAsync(context, argPos, _services);
                
                if (!result.IsSuccess)
                {
                    Console.WriteLine("An error occured");
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}