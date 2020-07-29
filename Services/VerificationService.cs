using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OsuFriendBot.Services
{
    class VerificationService
    {
        private readonly DiscordSocketClient _discord;
        private readonly ILogger _logger;

        public VerificationService(DiscordSocketClient discord, ILogger<VerificationService> logger)
        {
            _discord = discord;
            _logger = logger;

            _discord.UserJoined += UserJoinedAsync;
        }

        public async Task UserJoinedAsync(SocketGuildUser user)
        {
            // TODO: Check database before sending message
            await user.SendMessageAsync($"Hi {user.Username}! Verify your osu! account to get personalized roles.");
        }
    }
}
