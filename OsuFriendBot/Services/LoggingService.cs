using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace OsuFriendBot.Services
{
    public class LoggingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly ILogger _logger;

        // DiscordSocketClient and CommandService are injected automatically from the IServiceProvider
        public LoggingService(DiscordSocketClient discord, CommandService commands, ILogger<LoggingService> logger)
        {
            _discord = discord;
            _commands = commands;
            _logger = logger;
            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
        {
            string logText = $"[Discord] {msg.Exception?.ToString() ?? msg.Message}";
            switch (msg.Severity) // Translate discord net logging levels
            {
                case LogSeverity.Critical:
                    {
                        _logger.LogCritical(logText);
                        break;
                    }
                case LogSeverity.Error:
                    {
                        _logger.LogError(logText);
                        break;
                    }
                case LogSeverity.Warning:
                    {
                        _logger.LogWarning(logText);
                        break;
                    }
                case LogSeverity.Info:
                    {
                        _logger.LogInformation(logText);
                        break;
                    }
                case LogSeverity.Debug: // Switching debug with verbose because discord.net is stupid
                    {
                        _logger.LogTrace(logText);
                        break;
                    }
                case LogSeverity.Verbose:
                    {
                        _logger.LogDebug(logText);
                        break;
                    }
            }
            return Task.CompletedTask;
        }
    }
}