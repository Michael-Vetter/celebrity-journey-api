using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class PresignedPostUrlResponse
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("fields")]
        public Fields Fields { get; set; }
        [JsonProperty("filepath")]
        public string FilePath { get; set; }
    }

    public class Fields
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("acl")]
        public string Acl { get; set; }
        [JsonProperty("bucket")]
        public string Bucket { get; set; }
    }
}
