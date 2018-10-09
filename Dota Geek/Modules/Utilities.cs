using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Dota_Geek.Modules
{
    public class Utilities : ModuleBase<SocketCommandContext>
    {
        [Name("Utilities")]
        [Command("Ping", RunMode = RunMode.Async)]
        [Alias("latency", "pong", "rtt")]
        [Summary("Returns the current estimated round-trip latency over WebSocket and REST")]
        public async Task LatencyAsyncTask()
        {
            IUserMessage message;
            Stopwatch stopwatch;
            var heartbeat = Context.Client.Latency;

            var tcs = new TaskCompletionSource<long>();
            var timeout = Task.Delay(TimeSpan.FromSeconds(30));

            Task TestMessageAsync(SocketMessage arg)
            {
                if (arg.Id != message?.Id) return Task.CompletedTask;

                tcs.SetResult(stopwatch.ElapsedMilliseconds);
                return Task.CompletedTask;
            }

            stopwatch = Stopwatch.StartNew();
            message = await ReplyAsync($"Hearbeat: {heartbeat}ms: init: ---, rtt: ---");
            var init = stopwatch.ElapsedMilliseconds;

            Context.Client.MessageReceived += TestMessageAsync;
            var task = await Task.WhenAny(tcs.Task, timeout);
            Context.Client.MessageReceived -= TestMessageAsync;
            stopwatch.Stop();

            if (task == timeout)
            {
                await message.ModifyAsync(x => x.Content = $"Heartbeat: {heartbeat}ms, init: {init}ms, rtt: timed out");
            }
            else
            {
                var rtt = await tcs.Task;
                await message.ModifyAsync(x => x.Content = $"Heartbeat: {heartbeat}ms, init: {init}ms, rtt: {rtt}ms");
            }
        }

        [Command("Support")]
        [Summary("Returns the current estimated round-trip latency over WebSocket and REST")]
        public async Task SupportTask()
        {
            await Invite();
        }

        [Command("Vote")]
        [Summary("Returns the current estimated round-trip latency over WebSocket and REST")]
        public async Task VoteTask()
        {
            await Invite();
        }

        [Command("Invite")]
        [Summary("Returns the current estimated round-trip latency over WebSocket and REST")]
        public async Task InviteTask()
        {
            await Invite();
        }

        private async Task Invite()
        {
            var embed = new EmbedBuilder
            {
                Description =
                    "1. [Invite](https://discordapp.com/api/oauth2/authorize?client_id=485759803155546113&permissions=859307344&scope=bot)\n" +
                    "2. [Vote](https://discordbots.org/bot/485759803155546113/vote)\n" +
                    "3. [Support Server](https://discord.gg/8wa4TZ5)",
                ImageUrl = "https://discordbots.org/api/widget/485759803155546113.png"
            };
            await ReplyAsync(string.Empty, embed: embed.Build());
        }
    }
}