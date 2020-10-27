using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using OsuFriendsApi;
using OsuFriendsDb.Services;
using System.Threading.Tasks;

namespace OsuFriendsBot.Modules
{
    [RequireContext(ContextType.Guild)]
    [Name("Fun")]
    [Summary("Fun commands")]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        private readonly OsuFriendsClient _osuFriends;
        private readonly DbUserDataService _dbUserData;
        private readonly ILogger<FunModule> _logger;

        public FunModule(OsuFriendsClient osuFriends, DbUserDataService dbUserData, ILogger<FunModule> logger)
        {
            _osuFriends = osuFriends;
            _dbUserData = dbUserData;
            _logger = logger;
        }

        [Command("uwu")]
        [Summary("UwU")]
        public async Task UwuCmd()
        {
            int count = _dbUserData.FindById(Context.User.Id).Uwu;
            await ReplyAsync($"{Format.Bold("What's this?")} | {Context.User.Username}, you've {Format.Bold("uwu")}'d {Format.Code(count.ToString())} times");
        }
    }
}