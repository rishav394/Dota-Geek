using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Dota_Geek.Modules
{
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("latency", RunMode = RunMode.Async)]
        [Alias("ping", "pong", "rtt")]
        [Summary("Returns the current estimated round-trip latency over WebSocket and REST")]
        public async Task LatencyAsyncTask()
        {
            IUserMessage message;
            Stopwatch stopwatch;
            int heartbeat = Context.Client.Latency;

            var tcs = new TaskCompletionSource<long>();
            Task timeout = Task.Delay(TimeSpan.FromSeconds(30));

            Task TestMessageAsync(SocketMessage arg)
            {
                if (arg.Id != message?.Id)
                {
                    return Task.CompletedTask;
                }

                tcs.SetResult(stopwatch.ElapsedMilliseconds);
                return Task.CompletedTask;
            }

            stopwatch = Stopwatch.StartNew();
            message = await ReplyAsync($"Hearbeat: {heartbeat}ms: init: ---, rtt: ---");
            long init = stopwatch.ElapsedMilliseconds;

            Context.Client.MessageReceived += TestMessageAsync;
            Task task = await Task.WhenAny(tcs.Task, timeout);
            Context.Client.MessageReceived -= TestMessageAsync;
            stopwatch.Stop();

            if (task == timeout)
            {
                await message.ModifyAsync(x => x.Content = $"Heartbeat: {heartbeat}ms, init: {init}ms, rtt: timed out");
            }
            else
            {
                long rtt = await tcs.Task;
                await message.ModifyAsync(x => x.Content = $"Heartbeat: {heartbeat}ms, init: {init}ms, rtt: {rtt}ms");
            }
        }
    }
}