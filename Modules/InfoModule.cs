using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace OsuFriendBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    [RequireContext(ContextType.Guild)]
    [Name("Information module")]
    [Summary("Useful informations about me")]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;
        private readonly Config _config;
        private readonly ILogger _logger;

        public InfoModule(CommandService commands, Config config, ILogger<InfoModule> logger)
        {
            _commands = commands;
            _config = config;
            _logger = logger;
        }

        [Command("help")]
        [Summary("Shows all commands")]
        public async Task HelpCmd()
        {
            var modules = _commands.Modules.OrderBy(x => x.Name).ToList();
            foreach (var module in modules)
            {
                EmbedBuilder embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle(module.Name).WithDescription(module.Summary ?? "No summary");
                var commands = module.Commands;
                foreach (CommandInfo command in commands)
                {
                    string title = $"{string.Join(" | ", command.Aliases)}";
                    if (command.Summary != null)
                    {
                        title += $" | {command.Summary}";
                    }
                    string text = $"{_config.Prefix}{command.Name}";
                    foreach (var param in command.Parameters)
                    {
                        text += $" | {param.Name}";
                        if (param.Summary != null)
                        {
                            text += $": {param.Summary}";
                        }
                    }
                    text = Format.Code('\n' + text);
                    embedBuilder.AddField(title, text);
                }
                await ReplyAsync(embed: embedBuilder.Build());
            }
        }
    }
}