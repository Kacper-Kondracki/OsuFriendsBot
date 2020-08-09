using Discord.Commands;

namespace OsuFriendsBot.RuntimeResults
{
    public class VerificationResult : RuntimeResult
    {
        public VerificationResult(CommandError? error, string reason) : base(error, reason)
        {
        }

        public static VerificationResult FromError(string reason)
        {
            return new VerificationResult(CommandError.Unsuccessful, reason);
        }

        public static VerificationResult FromSuccess(string reason = null)
        {
            return new VerificationResult(null, reason);
        }
    }
}