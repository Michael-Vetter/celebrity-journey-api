using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class EventYear : IComparable
    {
        [JsonProperty("year")]
        public int Year { get; set; }
        [JsonProperty("months")]
        public List<EventMonth> Months { get; set; }


        public int CompareTo(object obj)
        {
            var thisYear = this.Year;
            var objYear = ((EventYear)obj).Year;

            if (thisYear == objYear)
                return 0;
            if (thisYear > objYear)
                return 1;

            return -1;
        }
    }
}
