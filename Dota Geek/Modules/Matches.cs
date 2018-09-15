using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using Dota_Geek.DataTypes;
using Dota_Geek.DataTypes.OpenDota;
using Newtonsoft.Json;

namespace Dota_Geek.Modules
{
    public class Matches : ModuleBase<SocketCommandContext>
    {
        public static string SteamApiKey { get; } = "902AC23891ED8519FFCDE9D49DC65725";

        [Command("profile", RunMode = RunMode.Async)]
        public async Task LastMatchTask(string accountId)
        {
            using (var client = new WebClient())
            {
                var url = $"https://api.opendota.com/api/players/{accountId.Steam32Parse()}";
                var json = client.DownloadString(url);
                PlayerProfile obj = JsonConvert.DeserializeObject<PlayerProfile>(json);
                

                var winLoss = WinTask(accountId);
                var win = winLoss.win;
                var lose = winLoss.lose;
                // TODO : something
            }

            await ReplyAsync("done");
        }

        [Command("match", RunMode = RunMode.Async)]
        [Alias("matches", "match data")]
        public async Task MatchTask(long matchId)
        {
            using (var client = new WebClient())
            {
                var url = $"https://api.opendota.com/api/matches/{matchId}";
                var json = client.DownloadString(url);
                var obj = IndividualMatchData.FromJson(json);

                var my = "```" +
                         "Player Name".PadRight(20) +
                         "Hero Name".PadRight(20) +
                         "Kills".PadRight(7) +
                         "Death".PadRight(7) +
                         "Assists".PadRight(7) +
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

                await ReplyAsync(my);
            }
        }

        private dynamic WinTask(string accountId)
        {
            using (var client = new WebClient())
            {
                var url = $"https://api.opendota.com/api/players/{accountId.Steam32Parse()}/wl";
                var json = client.DownloadString(url);
                var obj = JsonConvert.DeserializeObject<dynamic>(json);
                return obj;
            }
        }

        [Command("recent matches", RunMode = RunMode.Async)]
        public async Task RecentMatchesTask(string accountId)
        {
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

        [Command("teams")]
        public async Task ProTeamsTask()
        {
            using (var client = new WebClient())
            {
                var json = client.DownloadString("https://api.opendota.com/api/teams");
                var obj = JsonConvert.DeserializeObject<List<Teams>>(json);
                var sorted = obj.OrderByDescending(x => x.Rating).Take(10);

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

        [Command("hero ranking", RunMode = RunMode.Async)]
        public async Task HeroRankingTask(string accountId)
        {
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


// TODO : RECENT MATCHES AND TEAM ID