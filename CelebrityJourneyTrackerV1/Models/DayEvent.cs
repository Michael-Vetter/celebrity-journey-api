using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class DayEvent
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
    }
}