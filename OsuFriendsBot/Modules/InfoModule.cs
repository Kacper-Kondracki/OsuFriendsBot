using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.Rest;
using Microsoft.Extensions.Logging;
using OsuFriendsBot.Embeds;
using OsuFriendsDb.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OsuFriendsBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    [Name("Informations")]
    [Summary("Useful informations about me")]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly GuildSettingsCacheService _guildSettings;
        private readonly CommandService _commands;
        private readonly Config _config;
        private readonly ILogger _logger;

        public InfoModule(GuildSettingsCacheService guildSettings, CommandService commands, Config config, ILogger<InfoModule> logger)
        {
            _guildSettings = guildSettings;
            _commands = commands;
            _config = config;
            _logger = logger;
        }

        [Command("help")]
        [Summary("Shows all commands")]
        public async Task<RuntimeResult> HelpCmd()
        {
            List<ModuleInfo> modules = _commands.Modules.OrderBy(x => x.Name).ToList();
            string prefix = Context.Guild != null ? _guildSettings.GetOrAddGuildSettings(Context.Guild.Id).Prefix ?? _config.Prefix : _config.Prefix;
            try
            {
                foreach (ModuleInfo module in modules)
                {
                    await Context.User.SendMessageAsync(embed: new CommandsEmbed(module, prefix).Build());
                }
            }
            catch (HttpException e)
            {
                switch (e.DiscordCode)
                {
                    case 50007:
                        return RuntimeResults.HelpResult.FromError("Sorry, I can't send direct message to you, please check if you block DMs via server");
                    default:
                        throw;
                }
            }
            return RuntimeResults.HelpResult.FromSuccess();
        }

        [Command("help")]
        [Summary("Shows all commands")]
        public async Task HelpCmd(string command)
        {
            SearchResult search = _commands.Search(command);
            if (!search.IsSuccess)
            {
                return;
            }
            string prefix = Context.Guild != null ? _guildSettings.GetOrAddGuildSettings(Context.Guild.Id).Prefix ?? _config.Prefix : _config.Prefix;
            foreach (CommandMatch cmd in search.Commands)
            {
                await ReplyAsync(embed: new CommandEmbed(cmd.Command, prefix).Build());
            }
        }

        [Command("info")]
        [Alias("about")]
        [Summary("Information about bot")]
        public async Task InfoCmd()
        {
            RestApplication app = await Context.Client.GetApplicationInfoAsync();
            await ReplyAsync(embed: new InfoEmbed(app).Build());
        }
    }
}