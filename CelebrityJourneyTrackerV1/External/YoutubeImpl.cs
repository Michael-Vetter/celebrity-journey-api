using CelebrityJourneyTrackerV1.Configurations;
using CelebrityJourneyTrackerV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace CelebrityJourneyTrackerV1.External
{
    public interface IYoutubeImpl
    {
        Task<Youtube> getYoutubeData(string videoId);
    }

    public class YoutubeImplLive : IYoutubeImpl
    {
        private readonly YoutubeConfiguration _youtubeConfiguration;

        public YoutubeImplLive(YoutubeConfiguration youtubeConfiguration)
        {
            _youtubeConfiguration = youtubeConfiguration;
        }
        public async Task<Youtube> getYoutubeData(string videoId)
        {
            using (var httpClient = new HttpClient())
            {
                var url = $"{_youtubeConfiguration.Url}/videos?id={videoId}&key={_youtubeConfiguration.GoogleKey}&part=snippet,statistics";

                var response = await httpClient.GetAsync(url);
                var content = response.Content;
                var resultTemp = await response.Content.ReadAsStringAsync();
                var result = await response.Content.ReadAsAsync<Youtubes>();

                return result.Items.FirstOrDefault();
            }
        }
    }

    public class YoutubeImplMock : IYoutubeImpl
    {
        private readonly YoutubeConfiguration _youtubeConfiguration;

        public YoutubeImplMock(YoutubeConfiguration youtubeConfiguration)
        {
            _youtubeConfiguration = youtubeConfiguration;
        }

        public async Task<Youtube> getYoutubeData(string videoId)
        {
            return new Youtube
            {
                Kind = "test",
                Id = videoId,
                Etag = "test",
                Snippet = new YoutubeSnippet { },
                Statistics = new YoutubeStatistics { }
            };
        }
    }
}
