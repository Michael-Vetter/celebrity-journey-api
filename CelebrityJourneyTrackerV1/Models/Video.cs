using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class Video
    {
        [JsonProperty("videoId")]
        public string VideoId { get; set; }
    }
}
