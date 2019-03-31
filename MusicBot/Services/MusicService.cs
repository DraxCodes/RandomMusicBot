using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Heaven_s_Bot.Core.UserAccounts;
using MusicBot.Core.Entities;
using MusicBot.Core.Logger;
using MusicBot.Discord.EntityConverters;
using MusicBot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Victoria;
using Victoria.Entities;

namespace MusicBot.Services
{
    public class MusicService : IServiceExtension
    {
        private DiscordShardedClient _client;
        private LavaShardClient _lavaShardedClient;
        private LavaRestClient _lavaRestClient;
        private LavaPlayer _player;
        private ILogger _logger;

        private int _shardsReady = 0;

        public MusicService(DiscordShardedClient client, LavaShardClient LavaShardedClient, LavaRestClient lavaRestClient, ILogger logger)
        {
            _client = client;
            _lavaShardedClient = LavaShardedClient;
            _lavaRestClient = lavaRestClient;
            _logger = logger;
        }

        public Task InitializeAsync()
        {
            _client.ShardReady += _client_ShardReady;
            _lavaShardedClient.Log += LavaLogAsync;
            _lavaShardedClient.OnTrackFinished += LavaTrackFinishedAsync;
            _lavaShardedClient.OnServerStats += DisplayStatsAsync;
            return Task.CompletedTask;
        }

        private async Task _client_ShardReady(DiscordSocketClient sClient)
        {
            _shardsReady++;
            if (_shardsReady == _client.Shards.Count)
            {
                await Task.Delay(500);
                await _lavaShardedClient.StartAsync(_client);
                _shardsReady = 0;
            }
        }

        private Task DisplayStatsAsync(ServerStats stats)
        {
            _logger.LogAsync(stats.Uptime.ToString());
            return Task.CompletedTask;
        }

        private async Task LavaTrackFinishedAsync(LavaPlayer player, LavaTrack track, TrackEndReason reason)
        {
            if (!reason.ShouldPlayNext())
                return;
            
            if (!player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
            {
                await player.TextChannel?.SendMessageAsync($"There are no more items left in queue.");
                return;
            }

            await player.PlayAsync(nextTrack);
        }

        public async Task SetVolumeAsync(int vol)
        {
            if (_player != null && _player.IsPlaying)
            {
                await _player.SetVolumeAsync(vol);
            }
        }

        public async Task ConnectAsync(SocketVoiceChannel voiceChannel, ITextChannel textChannel)
            => await _lavaShardedClient.ConnectAsync(voiceChannel, textChannel);

        public async Task LeaveAsync()
            => await _lavaShardedClient.DisposeAsync();

        public async Task<MusicEmbed> PlayAsync(string search, ulong id)
        {
            _player = _lavaShardedClient.GetPlayer(id);
            var musicEmbed = new MusicEmbed();
            var results = await _lavaRestClient.SearchYouTubeAsync(search);
            if (results.LoadType == LoadType.NoMatches ||
                results.LoadType == LoadType.LoadFailed)
            {
                musicEmbed.Title = "Nothing to play.";
                return musicEmbed;
            }

            var track = results.Tracks.FirstOrDefault();
            if (_player.IsPlaying)
            {
                _player.Queue.Enqueue(track);
                musicEmbed.Title = $"Added To Queue: {track.Title}";
                musicEmbed.ImageUrl = await track.FetchThumbnailAsync();
                musicEmbed.Url = track.Uri.ToString();
                musicEmbed.Length = track.Length;
                musicEmbed.Author = track.Author;
                return musicEmbed;
            }
            else
            {
                await _player.PlayAsync(track);
                musicEmbed.Title = $"Now Playing: {track.Title}";
                musicEmbed.ImageUrl = await track.FetchThumbnailAsync();
                musicEmbed.Url = track.Uri.ToString();
                musicEmbed.Length = track.Length;
                musicEmbed.Author = track.Author;
                return musicEmbed;
            }
        }

        public async Task<string> StopAsync()
        {
            if (_player != null && _player.IsPlaying)
            {
                await _player.StopAsync();
                return "Player stopped.";
            }
            else
            {
                return "Nothing is playing.";
            }
        }

        public async Task<string> SkipAsync()
        {
            if (_player.Queue.Count == 0)
            {
                return "Nothing to skip.";
            }

            if (_player != null && _player.IsPlaying)
            {
                await _player.SkipAsync();
                return $"Song Skipped, Now Playing: {_player.CurrentTrack.Title}";
            }
            return "Nothing is currently playing.";
        }

        public async Task<string> PauseOrResumeAsync()
        {
            if(_player != null && _player.IsPlaying && !_player.IsPaused)
            {
                await _player.PauseAsync();
                return "Player is now paused.";
            }
            else
            {
                await _player.ResumeAsync();
                return "Player now resumed.";
            }
        }

        public IEnumerable<LavaTrack> GetQueue()
        {
            if (_player != null && _player.IsPlaying && _player.Queue.Count > 0)
            {
                var queue = _player.Queue.Items.Cast<LavaTrack>();
                return queue;
            }
            return new List<LavaTrack>();
        }

        private async Task LavaLogAsync(LogMessage logMessage)
        {
            var log = LogConverter.ConvertLog(logMessage);
            await _logger.LogMessageAsync(log);
        }
    }
}
