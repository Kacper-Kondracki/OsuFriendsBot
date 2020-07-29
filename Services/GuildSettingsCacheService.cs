using Microsoft.Extensions.Logging;
using OsuFriendBot.Models;
using System.Collections.Concurrent;

namespace OsuFriendBot.Services
{
    public class GuildSettingsCacheService
    {
        private readonly DbGuildSettingsService _dbGuildSettings;
        private readonly ILogger _logger;

        private readonly ConcurrentDictionary<ulong, GuildSettings> cache;

        public GuildSettingsCacheService(DbGuildSettingsService dbGuildSettings, ILogger<GuildSettingsCacheService> logger)
        {
            _dbGuildSettings = dbGuildSettings;
            _logger = logger;

            cache = new ConcurrentDictionary<ulong, GuildSettings>(_dbGuildSettings.FindAllToDict());
        }

        public GuildSettings GetOrAddGuildSettings(ulong id)
        {
            return cache.GetOrAdd(id, new GuildSettings { GuildId = id });
        }

        public void UpsertGuildSettings(GuildSettings settings)
        {
            cache.AddOrUpdate(settings.GuildId, settings, (k, v) => v = settings);
            _dbGuildSettings.Upsert(settings);
        }
    }
}