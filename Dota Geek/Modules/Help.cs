using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Dota_Geek.Modules
{
    /// <summary>Class containing code for help module</summary>
    /// >
    public class Help : ModuleBase
    {
        /// <summary>Gets the command service</summary>
        /// >
        private readonly CommandService _commands;

        /// <summary>Initializes a new instance of the <see cref="Help" /> class.</summary>
        /// <param name="service">The service. </param>
        public Help(CommandService service)
        {
            _commands = service;
        }

        /// <summary>Lists the commands.</summary>
        /// <returns>returns nothing</returns>
        /// <param name="commandOrModule">Optional Module or command</param>
        [Command("help", RunMode = RunMode.Async)]
        [Summary("Lists all the commands")]
        public async Task HelpAsync([Remainder] string commandOrModule = null)
        {
            if (commandOrModule != null)
            {
                await DetailedHelpAsync(commandOrModule.ToLower());
                return;
            }

            var builder = new EmbedBuilder();
            {
                builder.WithColor(new Color(87, 222, 127));
                builder.WithTimestamp(DateTimeOffset.UtcNow.UtcDateTime);
                builder.WithTitle($"Hey {Context.User.Username}, here is a list of all my commands.");
                builder.WithFooter(
                    f => f.WithText("Use `help [command-name]` or `help [module-name]` for more information"));
            }

            // Looping thorough every module
            foreach (var module in _commands.Modules.OrderBy(x => x.Name))
            {
                string fieldValue = null;

                // Looping through every command in the selected module
                foreach (var cmd in module.Commands.OrderBy(x => x.Name))
                {
                    // Just a basic report stuff to log missed summaries
                    if (string.IsNullOrWhiteSpace(cmd.Summary)) Console.WriteLine("No summary for " + cmd.Name);

                    // TODO: Boy I need a better way to stop the precondition check for these modules
                    if (!module.Name.ToLower().Equals("help") && !module.Name.ToLower().Equals("ping"))
                    {
                        var result = await cmd.CheckPreconditionsAsync(Context);
                        if (result.IsSuccess) fieldValue += $"{cmd.Aliases.First()}, ";
                    }
                }

                if (string.IsNullOrWhiteSpace(fieldValue))
                    continue;
                // FieldValue could be empty when the precondition is false.
                // But ofc its not so we are here. Creating a field in the embed.
                fieldValue = fieldValue.Substring(0, fieldValue.Length - 2);
                builder.AddField(
                    x =>
                    {
                        x.Name = $"\n💠 {module.Name}";
                        x.Value = $"{fieldValue}";
                        x.IsInline = false;
                    });
            }

            await ReplyAsync(string.Empty, false, builder.Build());
        }

        /// <summary> The rateLimits task </summary>
        /// <returns> The <see cref="Task" /> </returns>
        [Command("rateLimits")]
        [Summary("Shows the rate limits for different actions")]
        public async Task RateLimitsTask()
        {
            const string message =
                "```prolog\nREST:\n        POST Message |  5/5s    | per-channel\n      DELETE Message |  5/1s    | per-channel\n PUT/DELETE Reaction |"
                + "  1/0.25s | per-channel\n        PATCH Member |  10/10s  | per-guild\n   PATCH Member Nick |  1/1s    | per-guild\n      PATCH Username |"
                + "  2/3600s | per-account\n      |All Requests| |  50/1s   | per-account\nWS:\n     Gateway Connect |   1/5s   | per-account\n     Presence Update |"
                + "   5/60s  | per-session\n |All Sent Messages| | 120/60s  | per-session```";

            await Context.Channel.SendMessageAsync(message);
        }

        /// <summary> Gets more help on a command  </summary>
        /// <param name="command"> The command </param>
        /// <returns> The <see cref="Task" /> </returns>
        private async Task DetailedHelpAsync([Remainder] string command)
        {
            var moduleFound = _commands.Modules.Select(mod => mod.Name.ToLower()).ToList().Contains(command);
            if (moduleFound)
            {
                await DetailedModuleHelpAsync(command);
                return;
            }

            // `command` isn't a module for sure. Now checking if it is a command
            var result = _commands.Search(Context, command);
            if (!result.IsSuccess)
            {
                await ReplyAsync(
                    $"Sorry, I couldn't find a command called *{command}* <:darthvadar:461157568174358552>");
                return;
            }

            var builder = new EmbedBuilder
            {
                Color = new Color(87, 222, 127)
            };

            foreach (var cmd in result.Commands.Select(match => match.Command))
                builder.AddField(
                    x =>
                    {
                        x.Name = $"Help on *{command}* coming right up";
                        var temp = "None";
                        if (cmd.Aliases.Count != 1) temp = string.Join(", ", cmd.Aliases);

                        x.Value = "**Aliases**: " + temp;
                        temp = "`" + Config.Bot.PrefixDictionary[Context.Guild.Id] + command;
                        if (cmd.Parameters.Count != 0)
                            temp += " " + string.Join(
                                        " ",
                                        cmd.Parameters.Select(
                                            p => p.IsOptional ? "<" + p.Name + ">" : "[" + p.Name + "]"));

                        temp += "`";
                        x.Value += $"\n**Usage**: {temp}\n**Summary**: {cmd.Summary}";

                        x.IsInline = false;
                    });

            builder.WithFooter("Note: Parameters under `[]` are mandatory and the ones under `<>` are optional");

            await ReplyAsync(string.Empty, false, builder.Build());
        }

        /// <summary> The detailed module help async </summary>
        /// <param name="module"> The command </param>
        /// <returns> The <see cref="Task" /> </returns>
        private async Task DetailedModuleHelpAsync(string module)
        {
            var first = _commands.Modules.First(mod => mod.Name.ToLower() == module);
            var embed = new EmbedBuilder
            {
                Title = "List of commands under " + module.ToUpper() + " module",
                Description = string.Empty,
                Color = new Color(87, 222, 127)
            };
            embed.WithFooter("Use `help [command-name]` for more information on the command");
            foreach (var cmds in first.Commands) embed.Description += $"{cmds.Name}, ";

            embed.Description = embed.Description.Substring(0, embed.Description.Length - 2);
            await ReplyAsync(string.Empty, false, embed.Build());
        }
    }
}