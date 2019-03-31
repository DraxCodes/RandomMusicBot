using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MusicBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Discord
{
    public class CommandHandler : IServiceExtension
    {
        private readonly DiscordShardedClient _client;
        private readonly CommandService _cmdService;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services, DiscordShardedClient client, CommandService cmdService)
        {
            _client = client;
            _cmdService = cmdService;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _cmdService.AddModulesAsync(
                Assembly.GetExecutingAssembly(),
                _services);
            HookEvents();
        }

        private void HookEvents()
        {
            _client.MessageReceived += HandlerMessageAsync;
            //_cmdService.CommandExecuted += CommandExecutedAsync;
            _cmdService.Log += LogAsync;
        }

        private Task LogAsync(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }

        private async Task HandlerMessageAsync(SocketMessage socketMessage)
        {

            if (!(socketMessage is SocketUserMessage message)) return;
            var argPos = 0;

            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new ShardedCommandContext(_client, message);

            var result = await _cmdService.ExecuteAsync(
                    context: context,
                    argPos: argPos,
                    services: _services);
        }

        //private Task CommandExecutedAsync(Optional<CommandInfo> arg1, ICommandContext arg2, IResult arg3)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
