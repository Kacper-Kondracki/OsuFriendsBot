using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OsuFriendsApi;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OsuFriendsBot.Services
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly HttpClient _httpClient;
        private readonly CommandHandlingService _commandHandler;
        private readonly IServiceProvider _services;
        private readonly Config _config;
        private readonly ILogger<StartupService> _logger;

        // DiscordSocketClient, CommandService, and IConfigurationRoot are injected automatically from the IServiceProvider
        public StartupService(
            DiscordSocketClient discord,
            HttpClient httpClient,
            CommandHandlingService commandHandler,
            IServiceProvider services,
            Config config,
            ILogger<StartupService> logger)
        {
            _discord = discord;
            _httpClient = httpClient;
            _commandHandler = commandHandler;
            _services = services;
            _config = config;
            _logger = logger;
        }

        public async Task StartAsync()
        {
            // Wake up services
            _logger.LogInformation("Waking up Logger service");
            _services.GetRequiredService<LoggingService>();
            _logger.LogInformation("Waking up Verification service");
            _services.GetRequiredService<VerificationService>();

            _logger.LogInformation("Setting OsuFriends Api Token");
            if (string.IsNullOrWhiteSpace(_config.OsuFriendsApiToken))
            {
                throw new Exception("Please enter your api token into the `config.json` file found in the applications root directory.");
            }

            _services.GetRequiredService<OsuFriendsClient>().SetToken(_config.OsuFriendsApiToken);

            if (_config.OsuFriendsApiUrl != null)
            {
                _services.GetRequiredService<OsuFriendsClient>().Url = _config.OsuFriendsApiUrl;
            }
            _config.OsuFriendsApiToken = string.Empty;

            _logger.LogInformation("Setting Discord Bot token");
            if (string.IsNullOrWhiteSpace(_config.Token))
            {
                throw new Exception("Please enter your bot's token into the `config.json` file found in the applications root directory.");
            }
            await _discord.LoginAsync(TokenType.Bot, _config.Token);     // Login to discord
            _config.Token = string.Empty;                               // Clear token for security
            await _discord.StartAsync();                                // Connect to the websocket
            await _discord.SetGameAsync($"{_config.Prefix}help", type: ActivityType.Listening);

            await _commandHandler.InitializeAsync();
        }
    }
}