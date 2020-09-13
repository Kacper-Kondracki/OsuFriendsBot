using Discord.Commands;

namespace OsuFriendsBot.RuntimeResults
{
    public class VerificationLockError : RuntimeResult
    {
        public VerificationLockError() : base(CommandError.Unsuccessful, "Complete your first verification before starting next one")
        {
        }
    }
}