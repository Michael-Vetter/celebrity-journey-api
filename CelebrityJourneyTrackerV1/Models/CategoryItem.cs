using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class CategoryItem
    {
        [JsonProperty("category")]
        public string Category { get; set; }
    }
}
