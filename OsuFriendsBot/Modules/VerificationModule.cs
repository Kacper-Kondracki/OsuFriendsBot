using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OsuFriendsBot.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OsuFriendsBot.Modules
{
    [RequireContext(ContextType.Guild)]
    [Name("Verification")]
    [Summary("Verify your osu! account")]
    public class VerificationModule : ModuleBase<SocketCommandContext>
    {
        private readonly VerificationService _verification;
        private readonly ILogger _logger;

        public VerificationModule(VerificationService verification, ILogger<VerificationModule> logger)
        {
            _verification = verification;
            _logger = logger;
        }

        [Command("verify", RunMode = RunMode.Async)]
        [Alias("refresh")]
        [Summary("Verify your osu! account or refresh your roles")]
        public async Task VerifyCmd()
        {
            await _verification.VerifyAsync(Context.User as SocketGuildUser);
        }

        [Command("roles")]
        [Summary("Shows osu! roles")]
        public async Task RolesCmd()
        {
            System.Collections.Generic.IEnumerable<string> roles = VerificationService.FindAllRoles(Context.Guild.Roles).Select(role => role.Name);

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle("Configured Roles:");

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
            System.Collections.Generic.IEnumerable<string> allGuildRoles = VerificationService.FindAllRoles(Context.Guild.Roles).Select(role => role.Name.ToUpperInvariant());
            System.Collections.Generic.List<string> allRoles = VerificationService.AllRoles();
            System.Collections.Generic.IEnumerable<string> missingRoles = allRoles.Except(allGuildRoles, StringComparer.InvariantCultureIgnoreCase);

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle($"Missing roles:");

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
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CreateMissingRolesCmd()
        {
            System.Collections.Generic.IEnumerable<string> allGuildRoles = VerificationService.FindAllRoles(Context.Guild.Roles).Select(role => role.Name.ToUpperInvariant());
            System.Collections.Generic.List<string> allRoles = VerificationService.AllRoles();
            System.Collections.Generic.IEnumerable<string> missingRoles = allRoles.Except(allGuildRoles, StringComparer.InvariantCultureIgnoreCase);

            foreach (string role in missingRoles)
            {
                await Context.Guild.CreateRoleAsync(role, isMentionable: false);
                await Task.Delay(TimeSpan.FromMilliseconds(150));
            }

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle($"Created roles:");

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
            var guildRoles = VerificationService.FindAllRoles(Context.Guild.Roles);

            foreach (var role in guildRoles)
            {
                await role.DeleteAsync();
                await Task.Delay(TimeSpan.FromMilliseconds(150));
            }

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle($"Deleted roles:");

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