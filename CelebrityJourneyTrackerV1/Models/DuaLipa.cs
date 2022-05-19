using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class DuaLipa : IComparable
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("dateSort")]
        public string DateSort { get; set; }
        [JsonProperty("dateEvent")]
        public string DateEvent { get; set; }
        [JsonProperty("videoId")]
        public string VideoId { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("songs")]
        public List<SongItem> Songs { get; set; }

        
        public int CompareTo(object obj)
        {
            var thisDateSort = DateTime.Parse(this.DateSort);
            var objDateSort = DateTime.Parse(((DuaLipa)obj).DateSort);

            return DateTime.Compare(thisDateSort, objDateSort);
        }
    }
}
