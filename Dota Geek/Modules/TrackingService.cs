using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Dota_Geek.DataTypes;

namespace Dota_Geek.Modules
{
    public class TrackingService : ModuleBase<SocketCommandContext>
    {
        [Command("Untrack")]
        public async Task UnTrackTask(string steamId)
        {
            var steam = steamId.Parser();
            UserGuildRelation.UserGuildRelationDictionary.TryAdd(Context.User.Id, new Dictionary<ulong, List<long>>());
            var temp = UserGuildRelation.UserGuildRelationDictionary[Context.User.Id];
            temp.TryAdd(Context.Guild.Id, new List<long>());
            if (temp[Context.Guild.Id].Contains(steam.Uid))
            {
                temp[Context.Guild.Id].Remove(steam.Uid);
                TrackedAccounts.TrackDictionary.TryAdd(steam.Uid, new List<SendData>());
                var p = TrackedAccounts.TrackDictionary[steam.Uid];
                p.RemoveAll(x => x.GuildId == Context.Guild.Id);
                UserGuildRelation.Save();
                TrackedAccounts.Save();
                await ReplyAsync(
                    $"I have successfully removed **{steam.Name}** ({steamId}) from your track list on this server.");
            }
            else
            {
                await ReplyAsync($"You haven't tracked **{steam.Name}** ({steamId}) on this server.");
                UserGuildRelation.Save();
            }
        }

        [Command("track")]
        public async Task TrackTask(string steamId)
        {
            var steam = steamId.Parser();
            var steam32 = steam.Uid;

            if (!Context.User.IsPro())
            {
                if (UserGuildRelation.UserGuildRelationDictionary.ContainsKey(Context.User.Id))
                {
                    var temp = UserGuildRelation.UserGuildRelationDictionary[Context.User.Id];
                    temp.TryAdd(Context.Guild.Id, new List<long>());

                    foreach (var pair in temp)
                        if (pair.Key == Context.Guild.Id)
                        {
                            if (pair.Value.Any())
                            {
                                var data = ("[U:1:" + pair.Value.First() + "]").Parser();
                                await ReplyAsync(
                                    $"You are already tracking **{data.Name}** ({data.Uid}) and are not a pro member so I cant let you track more than one Steam Profile");
                                return;
                            }

                            pair.Value.Add(steam32);
                            break;
                        }
                }
                else
                {
                    var temp = new Dictionary<ulong, List<long>> {{Context.Guild.Id, new List<long> {steam32}}};
                    UserGuildRelation.UserGuildRelationDictionary.Add(Context.User.Id, temp);
                }
            }

            if (TrackedAccounts.TrackDictionary.ContainsKey(steam32))
            {
                // User is allowed to add track more IDs
                var temp = TrackedAccounts.TrackDictionary[steam32];
                foreach (var data in temp)
                {
                    if (data.GuildId != Context.Guild.Id) continue;
                    await ReplyAsync(
                        $"Someone is already tracking {steam.Name} ({steam32}) in {((ITextChannel) Context.Client.GetChannel(data.ChannelId)).Mention}");
                    if (!Context.User.IsPro())
                        UserGuildRelation.UserGuildRelationDictionary[Context.User.Id][Context.Guild.Id]
                            .Remove(steam32);

                    UserGuildRelation.Save();
                    TrackedAccounts.Save();
                    return;
                }

                // Steam Account ain't be tracking in here
                temp.Add(new SendData
                {
                    ChannelId = Context.Channel.Id,
                    GuildId = Context.Guild.Id
                });

                TrackedAccounts.TrackDictionary[steam32] = temp;
            }
            else
            {
                var temp = new List<SendData>
                {
                    new SendData
                    {
                        GuildId = Context.Guild.Id,
                        ChannelId = Context.Channel.Id
                    }
                };
                TrackedAccounts.TrackDictionary.Add(steam32, temp);
            }

            TrackedAccounts.Save();
            UserGuildRelation.Save();
            await ReplyAsync(
                $"I will be posting {steam.Name}'s ({steam32}) new matches here in this channel every Hour :tada:");
        }
    }
}