using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OsuFriendBot.Services;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OsuFriendBot
{
    internal class Startup
    {
        private readonly Config config;

        public Startup(string[] args)
        {
            string json = File.ReadAllText("config.json");
            config = JsonConvert.DeserializeObject<Config>(json);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/log.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .MinimumLevel.Is(config.MinimumLevel)
                .CreateLogger();
        }

        public async Task StartAsync()
        {
            using ServiceProvider services = ConfigureServices();
            await services.GetRequiredService<StartupService>().StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Singletons
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Debug }))
                .AddSingleton<HttpClient>()
                // Services
                .AddSingleton<StartupService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<VerificationService>()
                .AddSingleton<LoggingService>()
                .AddSingleton(new LiteDatabase(config.ConnectionString))
                // Transients
                // Config
                .AddLogging(configure => configure.AddSerilog())
                .AddSingleton(config)
                .BuildServiceProvider();
        }
    }
}