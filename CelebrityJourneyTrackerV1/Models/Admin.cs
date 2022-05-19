using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class Admin
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
