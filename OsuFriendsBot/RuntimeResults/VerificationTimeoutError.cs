using Discord.Commands;
using Discord.WebSocket;

namespace OsuFriendsBot.RuntimeResults
{
    public class VerificationTimeoutError : RuntimeResult
    {
        public VerificationTimeoutError(SocketGuild guild) : base(CommandError.Unsuccessful, $"Verification failed because it timeouted! Try again with 'verify' command on {guild.Name}")
        {
        }
    }
}