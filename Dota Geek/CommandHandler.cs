using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Dota_Geek
{
    internal class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            _client.MessageReceived += HandleCommandAsync;
            _client.JoinedGuild += Join;
            _client.LeftGuild += Left;
            _client.Ready += Ready;
        }

        private Task Ready()
        {
            Global.Client = _client;
            return Task.CompletedTask;
        }

        private Task Left(SocketGuild arg)
        {
            throw new NotImplementedException();
        }

        private Task Join(SocketGuild arg)
        {
            throw new NotImplementedException();
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
                var typing = context.Channel.EnterTypingState();
                var result = await _service.ExecuteAsync(context, argPos);
                typing.Dispose();
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason + $" at {context.Guild.Name}");
                    if (result.Error == CommandError.UnknownCommand)
                    {
                        var guildEmote = Emote.Parse("<:unknowscmd:461157571701506049>");
                        await msg.AddReactionAsync(guildEmote);
                    }
                    else
                    {
                        await context.Channel.SendMessageAsync(result.Error.ToString());
                    }
                }
            }
        }
    }
}