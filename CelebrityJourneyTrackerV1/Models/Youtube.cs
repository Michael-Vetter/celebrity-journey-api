using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class Youtube
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }
        [JsonProperty("etag")]
        public string Etag { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("snippet")]
        public YoutubeSnippet Snippet { get; set; }
        [JsonProperty("statistics")]
        public YoutubeStatistics Statistics { get; set; }
    }

    public class Youtubes
    {
        [JsonProperty("items")]
        public List<Youtube> Items { get; set; }
    }

    public class YoutubeStatistics
    {
        [JsonProperty("viewCount")]
        public string ViewCount { get; set; }
        [JsonProperty("likeCount")]
        public string LikeCount { get; set; }
        [JsonProperty("favoriteCount")]
        public string FavoriteCount { get; set; }
        [JsonProperty("commentCount")]
        public string CommentCount { get; set; }

    }

    public class YoutubeSnippet
    {
        [JsonProperty("publishedAt")]
        public string PublishedAt { get; set; }
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("thumbnails")]
        public Thumbnails Thumbnails { get; set; }

        [JsonProperty("channelTitle")]
        public string ChannelTitle { get; set; }
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }
        [JsonProperty("liveBroadcastContent")]
        public string LiveBroadcastContent { get; set; }
    }

    public class Thumbnails
    {
        [JsonProperty("default")]
        public ThumbnailImage Default { get; set; }
        [JsonProperty("medium")]
        public ThumbnailImage Medium { get; set; }
        [JsonProperty("high")]
        public ThumbnailImage High { get; set; }
        [JsonProperty("standard")]
        public ThumbnailImage Standard { get; set; }
        [JsonProperty("maxres")]
        public ThumbnailImage Maxres { get; set; }
    }

    public class ThumbnailImage
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
    }
}
