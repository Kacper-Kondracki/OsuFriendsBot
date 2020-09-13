using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsuFriendsBot.RuntimeResults
{
    public class VerificationUserIdError : RuntimeResult
    {
        public VerificationUserIdError() : base(CommandError.Unsuccessful, "Verification failed because unused ID couldn't be found")
        {
        }
    }
}
