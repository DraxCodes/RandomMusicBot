using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MusicBot.Discord
{
    public interface IMusicBotClient
    {
        Task InitializeClientAsync();
    }
}
