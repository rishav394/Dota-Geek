using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Dota_Geek.Modules
{
    [RequireOwner]
    public class OwnerCommands : ModuleBase<SocketCommandContext>
    {
        [Command("Give database")]
        [Summary("Very bad command. Dont ever use.")]
        public async Task GiveDatabaseTask()
        {
            var msg = await ReplyAsync("<a:loader:461159122575032331> Loading Database");
            ZipFile.CreateFromDirectory(@"Resources", @"Resources.zip", CompressionLevel.Optimal, true);
            await Context.Channel.SendFileAsync("Resources.zip");
            File.Delete("Resources.zip");
            await msg.DeleteAsync();
        }

        [Command("Make pro")]
        [Summary("Make someone pro user. Requires mention.")]
        public async Task ProTask(SocketUser socketUser)
        {
            var result = socketUser.MakePro();
            if (result)
                await ReplyAsync($"Congratzzz {socketUser.Mention}, you are now a pro member :tada:");
            else
                await ReplyAsync($"{socketUser.Username} is already a pro Member <a:thinkez:462208367121661955>");
        }

        [Command("Remove pro")]
        [Summary("Revoke pro membership from someone. Requires mention.")]
        public async Task RemoveProTask(SocketUser socketUser)
        {
            var result = socketUser.RemovePro();
            if (result)
                await ReplyAsync($"{socketUser.Mention}, you are no longer a pro member <:rip:462208313480708096>");
            else
                await ReplyAsync($"{socketUser.Username} was never pro <:kyloren:461284603873853460>");
        }

        [Command("bot stats")]
        [Summary("Shares the bot summary here")]
        public async Task BotMainStats()
        {
            var time = DateTime.Now - Process.GetCurrentProcess().StartTime;
            var upTime = "Bot Has Been Up For: ";

            if (time.Days > 0)
            {
                if (time.Hours <= 0 || time.Minutes <= 0)
                {
                    upTime += $"{time.Days} Day(s) and ";
                }
                else
                {
                    upTime += $"{time.Days} Day(s),";
                }
            }

            if (time.Hours > 0)
            {
                if (time.Minutes > 0)
                {
                    upTime += $" {time.Hours} Hour(s), ";
                }
                else
                {
                    upTime += $"{time.Hours} Hour(s) And ";
                }
            }

            if (time.Minutes > 0)
            {
                upTime += $"{time.Minutes} Minute(s)";
            }

            if (time.Seconds >= 0)
            {
                if (time.Hours > 0 || time.Minutes > 0)
                {
                    upTime += $" And {time.Seconds} Second(s)";
                }
                else
                {
                    upTime += $"{time.Seconds} Second(s)";
                }
            }

            var process = Process.GetCurrentProcess();
            var mem = process.PrivateMemorySize64;
            var memory = mem / 1024 / 1024;
            var totalUsers = Context.Client.Guilds.Sum(guild => guild.MemberCount);

            var builder = new EmbedBuilder();
            builder.WithTitle("Bot Stats:");
            builder.WithDescription("Gets Bot's stats");
            builder.WithThumbnailUrl(Context.User.GetAvatarUrl(ImageFormat.Auto, 64));
            builder.WithColor(new Color(0x00ff00));
            builder.AddField("Ping:", $"```fix\n{Context.Client.Latency}ms```", true);
            builder.AddField("Total Servers:", $"```fix\n{Context.Client.Guilds.Count} Servers```", true);
            builder.AddField("Total Users:", $"```fix\n{totalUsers} total users```", true);
            builder.WithTimestamp(DateTimeOffset.UtcNow.UtcDateTime);
            builder.WithFooter(
                x =>
                {
                    x.WithText($"Bot Stats | Requested by {Context.User.Username}");
                    x.WithIconUrl(Context.User.GetAvatarUrl());
                });
            if (Context.Client.GetGuild(458559328912408606).GetRole(458562677157920769).Members
                .Select(member => member.Id).Contains(Context.User.Id))
            {
                builder.AddField("Memory Usage:", $"```fix\n{memory}Mb```", true);
                builder.AddField("Up-time:", $"```prolog\n{upTime}```", true);
            }

            // Sends message and deletes
            await ReplyAsync(string.Empty, false, builder.Build());
        }
    }
}