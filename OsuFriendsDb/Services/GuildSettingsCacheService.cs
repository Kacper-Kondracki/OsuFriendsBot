using OsuFriendsDb.Models;
using System.Collections.Concurrent;

namespace OsuFriendsDb.Services
{
    public class GuildSettingsCacheService
    {
        private readonly DbGuildSettingsService _dbGuildSettings;

        private readonly ConcurrentDictionary<ulong, GuildSettings> cache;

        public GuildSettingsCacheService(DbGuildSettingsService dbGuildSettings)
        {
            _dbGuildSettings = dbGuildSettings;

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