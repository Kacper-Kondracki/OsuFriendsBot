using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OsuFriendsApi;
using OsuFriendsBot.Services;
using OsuFriendsDb.Services;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OsuFriendsBot
{
    internal class Startup
    {
        private readonly Config config;

        public Startup(string[] args)
        {
            string json = File.ReadAllText("config.json");
            config = JsonConvert.DeserializeObject<Config>(json);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(config.LogPath, rollingInterval: RollingInterval.Day)
                .WriteTo.Console(theme: AnsiConsoleTheme.Code, restrictedToMinimumLevel: config.MinimumLevel)
                .MinimumLevel.Is(LogEventLevel.Verbose)
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
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Info }))
                .AddSingleton<OsuFriendsClient>()
                .AddSingleton<HttpClient>()
                // Services
                .AddSingleton<StartupService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<VerificationService>()
                .AddSingleton<LoggingService>()
                // Database Stuff
                .AddSingleton(new LiteDatabase(config.ConnectionString))
                .AddSingleton<GuildSettingsCacheService>()
                // Transients
                .AddTransient<DbGuildSettingsService>()
                .AddTransient<DbUserDataService>()
                // Config
                .AddLogging(configure => configure.AddSerilog())
                .AddSingleton(config)
                .BuildServiceProvider();
        }
    }
}