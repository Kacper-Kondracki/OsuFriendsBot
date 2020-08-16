using Discord.Commands;

namespace OsuFriendsBot.RuntimeResults
{
    public class HelpResult : RuntimeResult
    {
        public HelpResult(CommandError? error, string reason) : base(error, reason)
        {
        }

        public static HelpResult FromError(string reason)
        {
            return new HelpResult(CommandError.Unsuccessful, reason);
        }

        public static HelpResult FromSuccess(string reason = null)
        {
            return new HelpResult(null, reason);
        }
    }
}