using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MusicBot.Discord;
using MusicBot.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MusicBot.Modules
{
    public class MusicModule : ModuleBase<ShardedCommandContext>
    {
        private MusicService _musicService;
        private DiscordShardedClient _client;

        public MusicModule(MusicService musicService, DiscordShardedClient client)
        {
            _musicService = musicService;
            _client = client;
        }

        [Command("Join")]
        public async Task Join()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel != null)
            {
                await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Now connected to: {user.VoiceChannel.Name}");
            }
            else
            {
                await ReplyAsync("Join a voice channel to use this command.");
                var chan = _client.GetChannel(377879473644765185) as SocketTextChannel;
                var msg = await chan.GetMessageAsync(559761812321468459);
                var author = msg.Author;
            }

        }

        [Command("Leave")]
        public async Task Leave()
            => await _musicService.LeaveAsync();

        [Command("Play")]
        public async Task Play([Remainder]string search)
        {
            var musicInfo = await _musicService.PlayAsync(search, Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithTitle(musicInfo.Title)
                .WithThumbnailUrl(musicInfo.ImageUrl)
                .WithDescription($"Author: {musicInfo.Author}\nLength: 0:00 - {musicInfo.Length:hh\\:mm\\:ss}") //TODO FIX THIS & Add check for hours.
                .WithUrl(musicInfo.Url);

            await ReplyAsync(embed: embed.Build());
        }

        [Command("Stop")]
        public async Task Stop()
            => await ReplyAsync(await _musicService.StopAsync());

        [Command("Skip")]
        public async Task Skip()
            => await ReplyAsync(await _musicService.SkipAsync());

        [Command("Pause"), Alias("Resume")]
        public async Task Pause()
            => await ReplyAsync(await _musicService.PauseOrResumeAsync());

        [Command("Volume")]
        public async Task test(int vol)
            => await _musicService.SetVolumeAsync(vol);

        [Command("List")]
        public async Task AddRole()
        {
            var queue = _musicService.GetQueue();
            if (queue.Count() is 0)
            {
                await ReplyAsync("Nothing in the queue");
                return;
            }

            var sb = new StringBuilder();
            foreach (var track in queue)
            {
                var title = string.Empty;
                if (track.Title.Length > 40)
                {
                    title = track.Title.Remove(40, (track.Title.Length - 40));
                    title += "...";
                }
                else
                {
                    title = track.Title;
                }

                sb.Append($"[{title}]({track.Uri})\n");
            }

            var embed = new EmbedBuilder()
                .WithTitle($"Current Queue: {queue.Count()} Items.")
                .WithDescription($"{sb}")
                .WithColor(Color.Blue)
                .WithCurrentTimestamp();
            await ReplyAsync(embed: embed.Build());
        }

        [Command("Test")]
        [RequireOwner]
        public async Task Test()
        {
            //The input
            var test = "kqtrwqkekqwjeyqwkyekljqwyelkqweliwqyelkwqehliqwyelkqwheqywelqwhelqweoiqwelqwleiuqweuqwe";

            //Regex way
            var regex = new Regex("[^e]");
            var newString = regex.Replace(test, "");
            Console.WriteLine(newString.Length);

            //Linq way
            var tetste = test.Where(x => x.Equals('e')).Count();
            Console.WriteLine(tetste);
        }
    }
}
