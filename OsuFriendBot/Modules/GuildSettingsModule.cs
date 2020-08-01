using Discord.Commands;
using Microsoft.Extensions.Logging;
using OsuFriendBot.Services;
using System.Threading.Tasks;

namespace OsuFriendBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    [RequireContext(ContextType.Guild)]
    [Name("Server settings")]
    [Summary("Change your server settings")]
    public class GuildSettingsModule : ModuleBase<SocketCommandContext>
    {
        private readonly GuildSettingsCacheService _guildSettings;
        private readonly Config _config;
        private readonly ILogger _logger;

        public GuildSettingsModule(GuildSettingsCacheService guildSettings, Config config, ILogger<GuildSettingsModule> logger)
        {
            _guildSettings = guildSettings;
            _config = config;
            _logger = logger;
        }

        [Command("prefix")]
        [Summary("Set custom bot prefix")]
        public async Task SetPrefix([Summary("If not specified, restores default prefix")] string prefix = null)
        {
            if (!string.IsNullOrEmpty(prefix) && prefix.Length > 32)
            {
                await ReplyAsync("Prefix can't be longer than 32 characters!"); // TODO: Use Post-Execution handler
                return;
            }
            Models.GuildSettings settings = _guildSettings.GetOrAddGuildSettings(Context.Guild.Id);
            settings.Prefix = prefix;
            _guildSettings.UpsertGuildSettings(settings);
            await ReplyAsync($"Current prefix: {prefix ?? _config.Prefix}");
        }
    }
}