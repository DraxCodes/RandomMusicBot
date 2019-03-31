using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MusicBot.Core.Entities.Logging;

namespace MusicBot.Core.Logger
{
    public class Logger : ILogger
    {
        public async Task LogAsync(string log)
        {
            var time = DateTime.UtcNow.ToString("HH:mm");
            await Append($"{time} ", ConsoleColor.DarkGray);
            await Append(log, ConsoleColor.Cyan);
        }

        public async Task LogMessageAsync(BotLog log)
        {
            var time = DateTime.UtcNow.ToString("HH:mm");
            await Append($"{time} ", ConsoleColor.DarkGray);
            await Append($"{log.Source} ", ConsoleColor.DarkGray);
            await Append($"[{log.Severity}] ", SeverityColor(log.Severity));
            await Append($"{log.Message}\n", ConsoleColor.White);
        }

        public async Task LogMessageExceptionAsync(BotLog log)
        {
            var time = DateTime.UtcNow.ToString("HH:mm");
            await Append($"{time} ", ConsoleColor.DarkGray);
            await Append($"{log.Source} ", ConsoleColor.DarkGray);
            await Append($"[{log.Severity}] ", SeverityColor(log.Severity));
            await Append($"{log.Message}\n", ConsoleColor.White);
            await Append($"{log.Message}", ConsoleColor.DarkGray);
        }

        private async Task Append(string message, ConsoleColor color)
        {
            await Task.Run(() => {
                Console.ForegroundColor = color;
                Console.Write(message);
                return Task.CompletedTask;
            });
        }

        private ConsoleColor SeverityColor(BotLogSeverity logSeverity)
        {
            switch (logSeverity)
            {
                case BotLogSeverity.CRIT:
                    return ConsoleColor.DarkRed;
                case BotLogSeverity.EROR:
                    return ConsoleColor.Red;
                case BotLogSeverity.WARN:
                    return ConsoleColor.Yellow;
                case BotLogSeverity.INFO:
                    return ConsoleColor.Green;
                default:
                    return ConsoleColor.Blue;
            }
        }
    }
}
