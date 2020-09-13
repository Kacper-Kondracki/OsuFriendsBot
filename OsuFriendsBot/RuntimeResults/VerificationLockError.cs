using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsuFriendsBot.RuntimeResults
{
    public class VerificationLockError : RuntimeResult
    {
        public VerificationLockError() : base(CommandError.Unsuccessful, "Complete your first verification before starting next one")
        {
        }
    }
}
