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
            _client.JoinedGuild += JoinAsync;
            _client.LeftGuild += LeftAsync;
        }

        private async Task LeftAsync(SocketGuild arg)
        {
            await Global.MeBot.Result.UpdateStatsAsync(Global.Client.Guilds.Count);
            var channel = Global.Client.GetGuild(480857253092524032).GetChannel(491281793039859723) as ITextChannel;
            if (channel is null) return;
            await channel.SendMessageAsync($"I just left {arg.Name} :slight_frown: ");
        }

        private async Task JoinAsync(SocketGuild arg)
        {
            await Global.MeBot.Result.UpdateStatsAsync(Global.Client.Guilds.Count);
            var channel = Global.Client.GetGuild(480857253092524032).GetChannel(491281793039859723) as ITextChannel;
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
                try
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
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
        }
    }
}