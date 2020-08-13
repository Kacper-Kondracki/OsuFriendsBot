using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OsuFriendsBot.Embeds;
using OsuFriendsBot.Osu.OsuFriendsBot.Services;
using OsuFriendsBot.RuntimeResults;
using OsuFriendsDb.Models;
using OsuFriendsDb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OsuFriendsBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    [RequireContext(ContextType.Guild)]
    [Name("Admin")]
    [Summary("Admin only commands")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private readonly GuildSettingsCacheService _guildSettingsCache;
        private readonly Config _config;
        private readonly ILogger _logger;

        public AdminModule(GuildSettingsCacheService guildSettingsCache, Config config, ILogger<AdminModule> logger)
        {
            _guildSettingsCache = guildSettingsCache;
            _config = config;
            _logger = logger;
        }

        // Server settings
        [Command("prefix")]
        [Summary("Set custom bot prefix")]
        public async Task<RuntimeResult> SetPrefixCmd([Summary("If not specified, restores default prefix")] string prefix = null)
        {
            if (!string.IsNullOrEmpty(prefix) && prefix.Length > 32)
            {
                return PrefixResult.FromError("Prefix can't be longer than 32 characters");
            }
            GuildSettings settings = _guildSettingsCache.GetOrAddGuildSettings(Context.Guild.Id);
            settings.Prefix = prefix;
            _guildSettingsCache.UpsertGuildSettings(settings);
            await ReplyAsync($"Current prefix: {prefix ?? _config.Prefix}");
            return PrefixResult.FromSuccess();
        }

        // Osu
        [Command("roles")]
        [Summary("Shows osu! roles")]
        public async Task RolesCmd()
        {
            List<string> roles = OsuRoles.FindAllRoles(Context.Guild.Roles).Select(role => role.Name).ToList();
            await ReplyAsync(embed: new RolesEmbed("Configured Roles:", roles).Build());
        }

        [Command("missingroles")]
        [Summary("Shows missing roles")]
        public async Task MissingRolesCmd()
        {
            IEnumerable<string> allGuildRoles = OsuRoles.FindAllRoles(Context.Guild.Roles).Select(role => role.Name.ToUpperInvariant());
            List<string> allRoles = OsuRoles.AllRoles();

            List<string> missingRoles = allRoles.Except(allGuildRoles, StringComparer.InvariantCultureIgnoreCase).ToList();
            await ReplyAsync(embed: new RolesEmbed("Missing Roles:", missingRoles).Build());
        }

        [Command("createmissingroles", RunMode = RunMode.Async)]
        [Summary("Creates missing roles")]
        public async Task CreateMissingRolesCmd()
        {
            IEnumerable<string> allGuildRoles = OsuRoles.FindAllRoles(Context.Guild.Roles).Select(role => role.Name.ToUpperInvariant());
            List<string> allRoles = OsuRoles.AllRoles();

            List<string> missingRoles = allRoles.Except(allGuildRoles, StringComparer.InvariantCultureIgnoreCase).ToList();

            foreach (string role in missingRoles)
            {
                await Context.Guild.CreateRoleAsync(role, isMentionable: false);
                await Task.Delay(TimeSpan.FromMilliseconds(150));
            }
            await ReplyAsync(embed: new RolesEmbed("Created Roles:", missingRoles).Build());
        }

        [Command("deletebotroles", RunMode = RunMode.Async)]
        [Summary("Delete osu! roles")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DeleteBotRolesCmd()
        {
            List<SocketRole> guildRoles = OsuRoles.FindAllRoles(Context.Guild.Roles);

            foreach (SocketRole role in guildRoles)
            {
                await role.DeleteAsync();
                await Task.Delay(TimeSpan.FromMilliseconds(150));
            }
            await ReplyAsync(embed: new RolesEmbed("Created Roles:", guildRoles.Select(role => role.Name).ToList()).Build());
        }
    }
}