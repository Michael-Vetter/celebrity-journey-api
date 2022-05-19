using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class DuaLipaVideo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("snippet")]
        public Snippet snippet { get; set; }
        [JsonProperty("statistics")]
        public Statistics Statistics { get; set; }
        [JsonProperty("dateOfEvent")]
        public string DateEvent { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("songs")]
        public List<SongItem> Songs { get; set; }
    }

    public class Snippet
    {
        [JsonProperty("publishedAt")]
        public string PublishedAt { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("thumbnails")]
        public Dictionary<string, Thumbnail> Thumbnails { get; set; }
    }

    public class Thumbnail
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Statistics
    {
        [JsonProperty("viewCount")]
        public string ViewCount { get; set; }
    }
}
