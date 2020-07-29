using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OsuFriendBot.Services
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly HttpClient _httpClient;
        private readonly CommandHandlingService _commandHandler;
        private readonly IServiceProvider _services;
        private readonly Config _config;

        // DiscordSocketClient, CommandService, and IConfigurationRoot are injected automatically from the IServiceProvider
        public StartupService(
            DiscordSocketClient discord,
            HttpClient httpClient,
            CommandHandlingService commandHandler,
            IServiceProvider services,
            Config config)
        {
            _discord = discord;
            _httpClient = httpClient;
            _commandHandler = commandHandler;
            _services = services;
            _config = config;
        }

        public async Task StartAsync()
        {
            // Wake up services
            _services.GetRequiredService<LoggingService>();
            _services.GetRequiredService<VerificationService>();

            string discordToken = _config.Token;
            if (string.IsNullOrWhiteSpace(discordToken))
            {
                throw new Exception("Please enter your bot's token into the `config.json` file found in the applications root directory.");
            }

            await _discord.LoginAsync(TokenType.Bot, discordToken);     // Login to discord
            _config.Token = string.Empty;                               // Clear token for security
            await _discord.StartAsync();                                // Connect to the websocket
            await _discord.SetGameAsync($"{_config.Prefix}help", type: ActivityType.Listening);

            await _commandHandler.InitializeAsync();
        }
    }
}