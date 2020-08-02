using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace OsuFriendsApi.Entities
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class OsuLevel
    {
        public OsuLevel(OsuLevelInfo level, double pp, long ppRank, long rankedScore, double hitAccuracy, long playCount, long playTime, long totalScore, long totalHits, long maximumCombo, long replaysWatchedByOthers, bool isRanked, OsuGradeCounts gradeCounts, OsuRank rank)
        {
            Level = level;
            PP = pp;
            PPRank = ppRank;
            RankedScore = rankedScore;
            HitAccuracy = hitAccuracy;
            PlayCount = playCount;
            PlayTime = TimeSpan.FromSeconds(playTime);
            TotalScore = totalScore;
            TotalHits = totalHits;
            MaximumCombo = maximumCombo;
            ReplaysWatchedByOthers = replaysWatchedByOthers;
            IsRanked = isRanked;
            GradeCounts = gradeCounts;
            Rank = rank;
        }

        public OsuLevelInfo Level { get; }
        public double PP { get; }
        public long PPRank { get; }
        public long RankedScore { get; }
        public double HitAccuracy { get; }
        public long PlayCount { get; }
        public TimeSpan PlayTime { get; }
        public long TotalScore { get; }
        public long TotalHits { get; }
        public long MaximumCombo { get; }
        public long ReplaysWatchedByOthers { get; }
        public bool IsRanked { get; }
        public OsuGradeCounts GradeCounts { get; }
        public OsuRank Rank { get; }
    }
}