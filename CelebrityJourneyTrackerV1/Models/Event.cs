using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class Event : IComparable
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("dateString")]
        public string DateString { get; set; }
        [JsonIgnore]
        public DateTime Date { get {
                if (DateTime.TryParse(DateString, out var eventDate))
                    return eventDate;
                return DateTime.MinValue;
            }  }
        [JsonProperty("org")]
        public string Org { get; set; }
        [JsonProperty("locality")]
        public string Locality { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("countryName")]
        public string CountryName { get; set; }

        public int CompareTo(object obj)
        {
            var thisEvent = this.Date;
            var objEvent = ((Event)obj).Date;

            if (thisEvent == objEvent)
                return 0;
            if (thisEvent > objEvent)
                return 1;

            return -1;
        }
    }
}
