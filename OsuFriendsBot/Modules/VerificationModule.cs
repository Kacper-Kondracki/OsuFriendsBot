using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OsuFriendsBot.Services;
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
            await _verification.Verify(Context.User as SocketGuildUser);
        }

        [Command("roles")]
        [Summary("Shows osu! roles")]
        public async Task RolesCmd()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle("Configured Roles:")
                .WithDescription(string.Join('\n', VerificationService.FindAllRoles(Context.Guild.Roles).Select(role => role.Name)));

            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}