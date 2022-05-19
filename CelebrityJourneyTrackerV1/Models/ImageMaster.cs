using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class ImageMaster
    {
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}
