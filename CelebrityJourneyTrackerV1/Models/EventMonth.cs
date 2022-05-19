using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class EventMonth : IComparable
    {
        private readonly List<string> monthText = new List<string> {
            "January" ,
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"};

        [JsonProperty("month")]
        public string Month { get; set; }
        [JsonProperty("events")]
        public List<Event> Events { get; set; }

        public int CompareTo(object obj)
        {
            var thisMonth = monthText.IndexOf(this.Month);
            var objMonth = monthText.IndexOf(((EventMonth)obj).Month);

            if (thisMonth == objMonth)
                return 0;
            if (thisMonth > objMonth)
                return 1;

            return -1;
        }
    }
}
