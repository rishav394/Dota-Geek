using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpLink;

namespace Dota_Geek
{
    // Should handle everything related to client and commands ONLY
    internal class SuperHandler
    {
        private readonly DiscordSocketClient _client;
        private CommandService _command;
        private IServiceProvider _services;
        private DiscordBotListHandler _discordBotListHandler;
        private LavalinkManager _lavalinkManager;

        public SuperHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task InitializeAsync()
        {
            _command = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            _lavalinkManager = new LavalinkManager(_client, new LavalinkManagerConfig
            {
                WebSocketHost = "127.0.0.1",
                WebSocketPort = 80,
                RESTHost = "127.0.0.1",
                RESTPort = 2333,
                Authorization = "youshallnotpass",
                TotalShards = 1,
                LogSeverity = LogSeverity.Verbose,
            });

            _discordBotListHandler = new DiscordBotListHandler(485759803155546113, Config.Bot.DblToken, _client);

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_command)
                .AddSingleton<InteractiveService>()
                .AddSingleton(_lavalinkManager)
                .BuildServiceProvider();

            await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.Ready += Ready;
            _client.MessageReceived += HandleCommandAsync;
            _client.JoinedGuild += JoinAsync;
            _client.LeftGuild += LeftAsync;
            _client.Log += Log;
            _lavalinkManager.Log += Log;
            _lavalinkManager.TrackEnd += TrackEnd;

        }
        
        private async Task TrackEnd(LavalinkPlayer player, LavalinkTrack track, string type)
        {
            if (type == "STOPPED")
            {
                return;
            }

            try
            {
                await player.PlayAsync(player.VoiceChannel.Guild.Id.PopTrack());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        private async Task Ready()
        {
            await _lavalinkManager.StartAsync();
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
            if (!(_client.GetGuild(480857253092524032).GetChannel(491281793039859723) is ITextChannel channel))
                return;
            await channel.SendMessageAsync($"I just joined {arg.Name} :tada:");
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg))
            {
                return;
            }

            var context = new SocketCommandContext(_client, msg);
            var argPos = 0;

            #region Dota Update

            var embed = s.Embeds.FirstOrDefault();
            if (s.Channel.Id == 437635625567649804 && s.Author.IsWebhook)
            {
                if (embed?.Author?.Name.StartsWith("Dota") == true)
                {
                    await BroadcastUpdate(embed);
                }
            }

            #endregion

            if (s.Author.IsBot)
            {
                return;
            }
                
            #region Prefix Management
            if (!Config.Bot.PrefixDictionary.ContainsKey(context.Guild.Id))
            {
                Config.Bot.PrefixDictionary.Add(context.Guild.Id, "$");
                Config.Save();
            }
            #endregion

            #region Command Management
            if (msg.HasStringPrefix(Config.Bot.PrefixDictionary[context.Guild.Id], ref argPos) ||
                msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                using (context.Channel.EnterTypingState())
                {

                    try
                    {
                        var result = await _command.ExecuteAsync(context, argPos, _services);
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
                                    await context.Channel.SendMessageAsync(
                                        "You are suppose to pass in a parameter with this" +
                                        " command. type `help [command name]` for help");
                                    break;
                                }
                                case CommandError.UnmetPrecondition:
                                {
                                    await context.Channel.SendMessageAsync(
                                        "You can not use this command at the moment.\nReason: " +
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
            #endregion
        }

        private async Task BroadcastUpdate(Embed embed)
        {
            foreach (var data in UpdateReceivers.Patches)
            {
                await _client.GetGuild(data.GuildId).GetTextChannel(data.ChannelId).SendMessageAsync(embed: embed);
            }
        }
    }
}