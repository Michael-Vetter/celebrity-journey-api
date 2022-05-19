using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CelebrityJourneyTrackerV1.External;
using CelebrityJourneyTrackerV1.Models;

namespace CelebrityJourneyTrackerV1.Services
{
    public interface IYoutubeApi
    {
        Task<Youtube> getYoutubeData(string videoId);
    }

    public class YoutubeApi : IYoutubeApi
    {
        private readonly IYoutubeImpl _youtubeImpl;

        public YoutubeApi(IYoutubeFactory youtubeFactory)
        {
            _youtubeImpl = youtubeFactory.getYoutube();
        }

        public async Task<Youtube> getYoutubeData(string videoId)
        {
            return await _youtubeImpl.getYoutubeData(videoId);
        }
    }
}
