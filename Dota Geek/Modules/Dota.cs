using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Dota_Geek.DataTypes;
using Dota_Geek.DataTypes.OpenDota;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dota_Geek.Modules
{
    public class Dota : ModuleBase<SocketCommandContext>
    {
        public static string SteamApiKey { get; } = "902AC23891ED8519FFCDE9D49DC65725";

        [Command("Profile", RunMode = RunMode.Async)]
        [Summary("Shows the target's current Profile with some nasty details")]
        public async Task LastMatchTask(string accountId = null)
        {
            if (accountId is null)
            {
                accountId = LinkedAccounts.UserDictionary.TryGetValue(Context.User.Id, out var longSteamId)
                    ? $"[U:1:{longSteamId}]"
                    : throw new ArgumentNullException();
            }

            var steam32Parse = accountId.Steam32Parse();
            var url = $"https://api.opendota.com/api/players/{steam32Parse}";
            using (var client = new WebClient())
            {
                var json = client.DownloadString(url);
                var obj = JsonConvert.DeserializeObject<PlayerProfile>(json);

                var embed = new EmbedBuilder();
                embed.WithAuthor(obj.Profile.Personaname ?? "Unknown", obj.Profile.Avatar, obj.Profile.Profileurl);
                
                var openDotoUrl = $"https://www.opendota.com/players/{obj.Profile.AccountId}";
                var dotaBuffUrl = $"https://www.dotabuff.com/players/{obj.Profile.AccountId}";
                if (obj.RankTier != null)
                    embed.AddField("Medal", ((int) (obj.RankTier / 10)).ParseMedal() + " " + obj.RankTier % 10, true);
                else
                    throw new ArgumentNullException($"Private Profile Probably.");

                var winLose = WinTask(steam32Parse.ToString());
                var win = winLose.win;
                var lose = winLose.lose;
                embed.AddField("Win/Loss", $"{win}/{lose} ({win * 100 / (win + lose)}%)", true);

                var url2 = $"https://api.opendota.com/api/players/{accountId.Steam32Parse()}/rankings";
                var json2 = client.DownloadString(url2);
                var obj2 = JsonConvert.DeserializeObject<List<HeroRankings>>(json2);
                var sorted = obj2.OrderByDescending(x => x.Score).Take(5);
                json2 = sorted.Aggregate("", (current, rankings) => current + rankings.HeroId.HeroName() + "\n");
                
                var file = File.ReadAllText("DataTypes\\Medals.json");
                var medal = JObject.Parse(file);
                embed.ThumbnailUrl = medal[obj.RankTier.ToString()].ToString();

                var url3 = $"https://api.opendota.com/api/players/{accountId.Steam32Parse()}/recentMatches";
                var json3 = client.DownloadString(url3);
                var recent = JsonConvert.DeserializeObject<List<RecentMatches>>(json3);
                var time = DateTimeOffset.FromUnixTimeSeconds(recent.First().StartTime);

                embed.AddField("Last Played", time.DateTime + " UTC", true);

                embed.AddField("Detailed stats", $"[OpenDota]({openDotoUrl})\n[DotaBuff]({dotaBuffUrl})", true);

                embed.AddField("Most Successful Heroes", json2, true);

                var r = new Random();
                embed.Color = new Color(r.Next(255), r.Next(255), r.Next(255));
                await ReplyAsync(string.Empty, false, embed.Build());
            }
        }

        [Command("Match", RunMode = RunMode.Async)]
        [Alias("matches", "match data")]
        [Summary("Gives you detailed analysis of a specific match")]
        public async Task MatchTask(long matchId)
        {
            var my = MatchDataGiver(matchId);
            await ReplyAsync(my);
        }

        private static string MatchDataGiver(long matchId)
        {
            string my;
            using (var client = new WebClient())
            {
                var url = $"https://api.opendota.com/api/matches/{matchId}";
                var json = client.DownloadString(url);
                var obj = IndividualMatchData.FromJson(json);

                my = "```" +
                         "Player Name".PadRight(20) +
                         "Hero Name".PadRight(20) +
                         "Kills".PadRight(7) +
                         "Death".PadRight(7) +
                         "Assist".PadRight(7) +
                         "XPM".PadRight(6) +
                         "GPM".PadRight(6) + "\n";
                for (var i = 0; i < 73; i++)
                    my += "_";
                my += "\n";
                foreach (var dataPlayer in obj.Players)
                    my += (dataPlayer.Personaname ?? "Unknown").Truncate(15).PadRight(20) +
                          dataPlayer.HeroId.HeroName().PadRight(20) +
                          dataPlayer.Kills.ToString().PadRight(7) +
                          dataPlayer.Deaths.ToString().PadRight(7) +
                          dataPlayer.Assists.ToString().PadRight(7) +
                          dataPlayer.XpPerMin.ToString().PadRight(6) +
                          dataPlayer.GoldPerMin.ToString().PadRight(6) +
                          "\n";

                my += "```";
            }

            return my;
        }

        private static dynamic WinTask(string accountId)
        {
            using (var client = new WebClient())
            {
                var url = $"https://api.opendota.com/api/players/{accountId}/wl";
                var json = client.DownloadString(url);
                var obj = JsonConvert.DeserializeObject<dynamic>(json);
                return obj;
            }
        }

        [Command("Recent Matches", RunMode = RunMode.Async)]
        [Summary("Gives you a list of the target's last 15 matches")]
        public async Task RecentMatchesTask(string accountId = null)
        {
            if (accountId is null)
            {
                accountId = LinkedAccounts.UserDictionary.TryGetValue(Context.User.Id, out var longSteamId)
                    ? $"[U:1:{longSteamId}]"
                    : throw new ArgumentNullException();
            }

            using (var client = new WebClient())
            {
                var url = $"https://api.opendota.com/api/players/{accountId.Steam32Parse()}/recentMatches";
                var json = client.DownloadString(url);
                var recent = JsonConvert.DeserializeObject<List<RecentMatches>>(json);

                var my =
                    $"```{"Name".PadRight(20, ' ') + "Kills".PadRight(8, ' ') + "Death".PadRight(8, ' ') + "Assist".PadRight(8, ' ') + "XPM".PadRight(8, ' ') + "GPM".PadRight(8, ' ') + "MatchID".PadRight(8, ' ')}";
                for (var i = 1; i <= 67; i++) my += "_";
                my += "\n";
                foreach (var recentMatches in recent)
                    my += recentMatches.HeroId.HeroName().PadRight(20, ' ')
                          + recentMatches.Kills.ToString().PadRight(8, ' ')
                          + recentMatches.Deaths.ToString().PadRight(8, ' ')
                          + recentMatches.Assists.ToString().PadRight(8, ' ')
                          + recentMatches.XpPerMin.ToString().PadRight(8, ' ')
                          + recentMatches.GoldPerMin.ToString().PadRight(8, ' ')
                          + recentMatches.MatchId
                          + "\n";

                my += "```";
                await ReplyAsync(my);
            }
        }

        [Command("Teams", RunMode = RunMode.Async), Alias("Pro")]
        [Summary("Gives you a list of top professional teams sorted by their ratings.")]
        public async Task ProTeamsTask()
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString("https://api.opendota.com/api/teams");
                var obj = JsonConvert.DeserializeObject<List<Teams>>(json);
                var sorted = obj.OrderByDescending(x => x.Rating).Take(15);

                var my =
                    $"```{"Name".PadRight(20, ' ') + "Wins".PadRight(10, ' ') + "Losses".PadRight(10, ' ') + "TeamID"}\n";
                for (var i = 1; i <= 50; i++) my += "_";

                my += "\n";
                my = sorted.Aggregate(my,
                    (current, team) => current + team.Name.PadRight(20, ' ') + team.Wins.ToString().PadRight(10, ' ') +
                                       team.Losses.ToString().PadRight(10, ' ') + team.TeamId + "\n");
                my += "```";
                await ReplyAsync(my);
            }
        }

        [Command("Hero ranking", RunMode = RunMode.Async)]
        [Summary("Gives you the target's hero list with specific performance.")]
        public async Task HeroRankingTask(string accountId = null)
        {
            if (accountId is null)
            {
                accountId = LinkedAccounts.UserDictionary.TryGetValue(Context.User.Id, out var longSteamId)
                    ? $"[U:1:{longSteamId}]"
                    : throw new ArgumentNullException();
            }

            using (var client = new WebClient())
            {
                var url = $"https://api.opendota.com/api/players/{accountId.Steam32Parse()}/rankings";
                var json = client.DownloadString(url);
                var obj = JsonConvert.DeserializeObject<List<HeroRankings>>(json);
                var sorted = obj.OrderByDescending(x => x.Score).Take(15);

                var url2 = $"https://api.opendota.com/api/players/{accountId.Steam32Parse()}/heroes";
                json = client.DownloadString(url2);
                var obj2 = JsonConvert.DeserializeObject<List<HeroPlayData>>(json);

                var my =
                    $"```{"Name".PadRight(20, ' ') + "Total".PadRight(10, ' ') + "Wins".PadRight(10, ' ') + "Losses".PadRight(10, ' ') + "Last Played UTC"}\n";
                for (var i = 1; i <= 70; i++) my += "_";

                my += "\n";
                foreach (var heroRankings in sorted)
                {
                    var heroPlayData = obj2.First(x => x.HeroId == heroRankings.HeroId);
                    var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddSeconds(heroPlayData.LastPlayed).ToLocalTime();
                    my += heroRankings.HeroId.HeroName().PadRight(20, ' ') +
                          heroPlayData.Games.ToString().PadRight(10, ' ') +
                          heroPlayData.Win.ToString().PadRight(10, ' ') +
                          (heroPlayData.Games - heroPlayData.Win).ToString().PadRight(10, ' ') +
                          dtDateTime + "\n";
                }

                my += "```";

                await ReplyAsync(my);
            }
        }
    }
}