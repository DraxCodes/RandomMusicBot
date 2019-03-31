using MusicBot.Core.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Core.Logger
{
    public interface ILogger
    {
        Task LogMessageAsync(BotLog log);
        Task LogMessageExceptionAsync(BotLog log);
        Task LogAsync(string log);
    }
}
