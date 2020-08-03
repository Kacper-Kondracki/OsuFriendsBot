using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OsuFriendsDb.Services;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace OsuFriendsBot.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly GuildSettingsCacheService _guildSettings;
        private readonly Config _config;
        private readonly IServiceProvider _services;
        private readonly ILogger<CommandHandlingService> _logger;

        public CommandHandlingService(DiscordSocketClient discord, CommandService commands, GuildSettingsCacheService guildSettings, Config config, IServiceProvider services, ILogger<CommandHandlingService> logger)
        {
            _discord = discord;
            _commands = commands;
            _guildSettings = guildSettings;
            _config = config;
            _services = services;
            _logger = logger;

            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            _discord.MessageReceived += MessageReceivedAsync;
            // Hook CommandExecuted to handle post-command-execution logic.
            _commands.CommandExecuted += CommandExecutedAsync;
        }

        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            // This value holds the offset where the prefix ends
            int argPos = 0;
            // Perform prefix check. You may want to replace this with
            // (!message.HasCharPrefix('!', ref argPos))
            // for a more traditional command format like !help.
            ulong? guildId = (message.Channel as SocketGuildChannel)?.Guild?.Id;

            if (!(message.HasStringPrefix(guildId != null ? _guildSettings.GetOrAddGuildSettings(guildId.Value).Prefix ?? _config.Prefix : _config.Prefix, ref argPos)
                || message.HasMentionPrefix(_discord.CurrentUser, ref argPos)))
            {
                return;
            }

            SocketCommandContext context = new SocketCommandContext(_discord, message);
            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            await _commands.ExecuteAsync(context, argPos, _services);
            // Note that normally a result will be returned by this format, but here
            // we will handle the result in CommandExecutedAsync,
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
            {
                return;
            }

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
            {
                return;
            }
            
            // the command failed, let's notify the user that something happened.
            if (result.ErrorReason == "The server responded with error 50007: Cannot send messages to this user")
            {
                await context.Channel.SendMessageAsync("Error: Please turn on your DMs for the server.");
            }
            else
            {
                await context.Channel.SendMessageAsync($"Error: {result.ErrorReason}");
            }
        }
    }
}