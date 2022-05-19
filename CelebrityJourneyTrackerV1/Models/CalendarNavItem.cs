using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class CalendarNavItem
    {
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("nodes")]
        public Dictionary<string, CalendarNavItem> Nodes { get; set; }
    }
}
