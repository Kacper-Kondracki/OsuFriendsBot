using Discord.Commands;

namespace OsuFriendsBot.RuntimeResults
{
    public class DirectMessageError : RuntimeResult
    {
        public DirectMessageError() : base(CommandError.Unsuccessful, "Sorry, I can't send direct message to you, please check if you block DMs via server")
        {
        }
    }
}