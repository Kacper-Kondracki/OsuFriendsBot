using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OsuFriendsBot.Services;
using OsuFriendsDb.Services;
using System.Threading.Tasks;

namespace OsuFriendsBot.Modules
{
    [RequireContext(ContextType.Guild)]
    [Name("Fun")]
    [Summary("Fun commands")]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        private readonly DbUserDataService _dbUserData;
        private readonly ILogger _logger;

        public FunModule(DbUserDataService dbUserData, ILogger<FunModule> logger)
        {
            _dbUserData = dbUserData;
            _logger = logger;
        }
        [Command("uwu")]
        [Summary("UwU")]
        public async Task UwuCmd()
        {
            var count = _dbUserData.FindById(Context.User.Id).Uwu;
            await ReplyAsync($"{Format.Bold("What's this?")} | {Context.User.Username}, you've {Format.Bold("uwu")}'d {Format.Code(count.ToString())} times");
        }
    }
}