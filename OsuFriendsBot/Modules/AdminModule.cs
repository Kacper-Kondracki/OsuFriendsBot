using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using OsuFriendsBot.Osu.OsuFriendsBot.Services;
using OsuFriendsDb.Models;
using OsuFriendsDb.Services;
using System;
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
        public async Task SetPrefixCmd([Summary("If not specified, restores default prefix")] string prefix = null)
        {
            if (!string.IsNullOrEmpty(prefix) && prefix.Length > 32)
            {
                await ReplyAsync("Prefix can't be longer than 32 characters!"); // TODO: Use Post-Execution handler
                return;
            }
            GuildSettings settings = _guildSettingsCache.GetOrAddGuildSettings(Context.Guild.Id);
            settings.Prefix = prefix;
            _guildSettingsCache.UpsertGuildSettings(settings);
            await ReplyAsync($"Current prefix: {prefix ?? _config.Prefix}");
        }

        // Osu
        [Command("roles")]
        [Summary("Shows osu! roles")]
        public async Task RolesCmd()
        {
            System.Collections.Generic.IEnumerable<string> roles = OsuRoles.FindAllRoles(Context.Guild.Roles).Select(role => role.Name);

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle("Configured Roles:")
                .WithColor(Color.Blue);

            if (roles.Any())
            {
                embedBuilder.WithDescription(string.Join('\n', roles));
            }
            else
            {
                embedBuilder.WithDescription("None");
            }

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("missingroles")]
        [Summary("Shows missing roles")]
        public async Task MissingRolesCmd()
        {
            System.Collections.Generic.IEnumerable<string> allGuildRoles = OsuRoles.FindAllRoles(Context.Guild.Roles).Select(role => role.Name.ToUpperInvariant());
            System.Collections.Generic.List<string> allRoles = OsuRoles.AllRoles();
            System.Collections.Generic.IEnumerable<string> missingRoles = allRoles.Except(allGuildRoles, StringComparer.InvariantCultureIgnoreCase);

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle($"Missing roles:")
                .WithColor(Color.Blue);

            if (missingRoles.Any())
            {
                embedBuilder.WithDescription(string.Join('\n', missingRoles));
            }
            else
            {
                embedBuilder.WithDescription("None");
            }

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("createmissingroles", RunMode = RunMode.Async)]
        [Summary("Creates missing roles")]
        public async Task CreateMissingRolesCmd()
        {
            System.Collections.Generic.IEnumerable<string> allGuildRoles = OsuRoles.FindAllRoles(Context.Guild.Roles).Select(role => role.Name.ToUpperInvariant());
            System.Collections.Generic.List<string> allRoles = OsuRoles.AllRoles();
            System.Collections.Generic.IEnumerable<string> missingRoles = allRoles.Except(allGuildRoles, StringComparer.InvariantCultureIgnoreCase);

            foreach (string role in missingRoles)
            {
                await Context.Guild.CreateRoleAsync(role, isMentionable: false);
                await Task.Delay(TimeSpan.FromMilliseconds(150));
            }

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle($"Created roles:")
                .WithColor(Color.Blue);

            if (missingRoles.Any())
            {
                embedBuilder.WithDescription(string.Join('\n', missingRoles));
            }
            else
            {
                embedBuilder.WithDescription("None");
            }

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("deletebotroles", RunMode = RunMode.Async)]
        [Summary("Delete osu! roles")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DeleteBotRolesCmd()
        {
            System.Collections.Generic.List<Discord.WebSocket.SocketRole> guildRoles = OsuRoles.FindAllRoles(Context.Guild.Roles);

            foreach (Discord.WebSocket.SocketRole role in guildRoles)
            {
                await role.DeleteAsync();
                await Task.Delay(TimeSpan.FromMilliseconds(150));
            }

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle($"Deleted roles:")
                .WithColor(Color.Blue);

            if (guildRoles.Any())
            {
                embedBuilder.WithDescription(string.Join('\n', guildRoles));
            }
            else
            {
                embedBuilder.WithDescription("None");
            }

            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}