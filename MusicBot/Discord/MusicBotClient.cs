using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;
using MusicBot.Storage;
using MusicBot.Core;
using MusicBot.Storage.Entities;
using MusicBot.Core.Logger;
using MusicBot.Discord.EntityConverters;
using System;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using MusicBot.Extensions;
using MusicBot.Services;
using Victoria;
using System.Collections.Generic;
using System.Linq;

namespace MusicBot.Discord
{
    public class MusicBotClient : IMusicBotClient
    {
        private DiscordShardedClient _client;
        private CommandService _cmdService;
        private IStorage _storage;
        private ILogger _logger;
        private IServiceProvider _services;

        private DiscordSocketConfig Config = new DiscordSocketConfig
        {
            AlwaysDownloadUsers = true,
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 50,
            TotalShards = 3
        };


        public MusicBotClient()
        {
            _cmdService = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Verbose
            });

            _storage = new PersistentStorage();
            _logger = new Logger();
        }

        public async Task InitializeClientAsync()
        {
            var botConfig = await GetOrCreateBotConfig();
            _services = SetupServices(Config);
            _client = _services.GetRequiredService<DiscordShardedClient>();

            await _client.LoginAsync(TokenType.Bot, botConfig.DiscordToken);
            await _client.StartAsync();

            await _services.InitializeServicesAsync();

            _client.Log += ClientLogAsync;

            await Task.Delay(-1);
        }

        private async Task ClientLogAsync(LogMessage logMessage)
        {
            var log = LogConverter.ConvertLog(logMessage);
            await _logger.LogMessageAsync(log);
        }

        private async Task<BotConfig> GetOrCreateBotConfig()
        {
            var botConfigPath = $"{Globals.ResourcesDirectory}/{Globals.BotConfigPath}";
            if (!_storage.FileExists(botConfigPath))
            {
                _storage.InitializeResources(Globals.ResourcesDirectory);
                _storage.CreatePath(botConfigPath);

                var botConfig = new BotConfig {
                    DiscordToken = "CHANGE ME",
                    GameStatus = "CHANGE ME",
                    Prefix = "!"
                };

                await _storage.Store(botConfig, botConfigPath);
            }

            return await _storage.Retreive<BotConfig>(botConfigPath);
        }

        private IServiceProvider SetupServices(DiscordSocketConfig config)
            => new ServiceCollection()
            .AddSingleton(new DiscordShardedClient(config))
            .AddSingleton(_cmdService)
            .AddSingleton(_logger)
            .AddSingleton(_storage)
            .AddSingleton<CommandHandler>()
            .AddSingleton<MusicService>()
            .AddSingleton<LavaRestClient>()
            .AddSingleton<LavaShardClient>()
            .BuildServiceProvider();
    }
}
