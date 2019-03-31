using System;

namespace MusicBot.Core.Entities.Logging
{
    public class BotLog
    {
        public string Source { get; set; }
        public string Message { get; set; }
        public BotLogSeverity Severity { get; set; }
        public Exception Exception { get; set; } = null;
    }
}
