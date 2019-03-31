using Discord;
using MusicBot.Core.Entities.Logging;

namespace MusicBot.Discord.EntityConverters
{
    public static class LogConverter
    {
        public static BotLog ConvertLog(LogMessage logMessage)
        {
            var botLog = new BotLog
            {
                Source = logMessage.Source,
                Message = logMessage.Message,
                Severity = SeverityColor(logMessage.Severity)
            };

            if (logMessage.Exception != null)
            {
                botLog.Exception = logMessage.Exception;
            }

            return botLog;
        }

        private static BotLogSeverity SeverityColor(LogSeverity logSeverity)
        {
            switch (logSeverity)
            {
                case LogSeverity.Critical:
                    return BotLogSeverity.CRIT;
                case LogSeverity.Error:
                    return BotLogSeverity.EROR;
                case LogSeverity.Warning:
                    return BotLogSeverity.WARN;
                case LogSeverity.Info:
                    return BotLogSeverity.INFO;
                case LogSeverity.Verbose:
                    return BotLogSeverity.INFO;
                case LogSeverity.Debug:
                    return BotLogSeverity.INFO;
                default:
                    return BotLogSeverity.INFO;
            }
        }
    }
}
