using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsuFriendsBot.RuntimeResults
{
    public class PrefixLengthError : RuntimeResult
    {
        public PrefixLengthError() : base(CommandError.Unsuccessful, "Prefix can't be longer than 32 characters")
        {
        }
    }
}
