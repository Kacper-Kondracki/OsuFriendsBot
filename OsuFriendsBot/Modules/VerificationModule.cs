using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OsuFriendsBot.Services;
using System.Threading.Tasks;

namespace OsuFriendsBot.Modules
{
    [RequireContext(ContextType.Guild)]
    [Name("Verification")]
    [Summary("Verify your osu! account")]
    public class VerificationModule : ModuleBase<SocketCommandContext>
    {
        private readonly VerificationService _verification;
        private readonly ILogger<VerificationModule> _logger;

        public VerificationModule(VerificationService verification, ILogger<VerificationModule> logger)
        {
            _verification = verification;
            _logger = logger;
        }

        [Command("verify", RunMode = RunMode.Async)]
        [Alias("refresh")]
        [Summary("Verify your osu! account or refresh your roles")]
        public async Task<RuntimeResult> VerifyCmd()
        {
            return await _verification.VerifyAsync(Context.User as SocketGuildUser, Context);
        }
    }
}