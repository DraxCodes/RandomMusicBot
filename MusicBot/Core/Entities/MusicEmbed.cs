using System;
using System.Collections.Generic;
using System.Text;

namespace MusicBot.Core.Entities
{
    public class MusicEmbed
    {
        public string Title { get; set; }
        public string Url { get; set; } = null;
        public string ImageUrl { get; set; } = null;
        public TimeSpan Length { get; set; }
        public string Author { get; set; }
    }
}
