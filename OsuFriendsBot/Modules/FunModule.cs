using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OsuFriendsApi;
using OsuFriendsDb.Services;
using System.Linq;
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

        [Command("party")]
        [Summary("party")]
        public async Task PartyCmd()
        {
            System.Collections.Generic.IReadOnlyCollection<OsuFriendsApi.Entities.Party> parties = await _osuFriends.GetPartiesAsync();
            await ReplyAsync(JsonConvert.SerializeObject(parties));
            OsuFriendsApi.Entities.Map x = (await _osuFriends.GetMappoolAsync(parties.First())).First();
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder
                .WithTitle(x.Name)
                .WithDescription(x.Url.ToString())
                .AddField("Difficulty", x.Difficulty)
                .AddField("Stars", x.Stars)
                .AddField("Status", x.Status)
                .AddField("BPM", x.Bpm)
                .WithImageUrl(x.Image.ToString());

            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}