using System;
using System.Collections.Generic;
using System.Text;

namespace MusicBot.Storage.Entities
{
    public class BotConfig
    {
        public string DiscordToken { get; set; }
        public string GameStatus { get; set; }
        public string Prefix { get; set; }
    }
}
