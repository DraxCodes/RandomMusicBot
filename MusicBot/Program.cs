using MusicBot.Discord;
using System;
using System.Threading.Tasks;

namespace MusicBot
{
    public class Program
    {
        public static async Task Main(string[] args = null)
            => await new MusicBotClient().InitializeClientAsync();
    }
}
