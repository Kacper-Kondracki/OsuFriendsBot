using Discord.Commands;

namespace OsuFriendsBot.RuntimeResults
{
    public class PrefixResult : RuntimeResult
    {
        public PrefixResult(CommandError? error, string reason) : base(error, reason)
        {
        }

        public static PrefixResult FromError(string reason)
        {
            return new PrefixResult(CommandError.Unsuccessful, reason);
        }

        public static PrefixResult FromSuccess(string reason = null)
        {
            return new PrefixResult(null, reason);
        }
    }
}