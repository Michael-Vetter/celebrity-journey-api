using System;
using CelebrityJourneyTrackerV1.Configurations;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CelebrityJourneyTrackerV1.External
{
    public interface IYoutubeFactory
    {
        IYoutubeImpl getYoutube();
    }

    public class YoutubeFactory : IYoutubeFactory
    {
        private readonly YoutubeConfiguration _youtubeConfiguration;
        private readonly IYoutubeImpl youtubeImpl;

        public YoutubeFactory(IOptions<YoutubeConfiguration> youtubeConfiguration)
        {
            _youtubeConfiguration = youtubeConfiguration.Value;
            if (_youtubeConfiguration.Url == "test")
                youtubeImpl = new YoutubeImplMock(_youtubeConfiguration);
            else
                youtubeImpl = new YoutubeImplLive(_youtubeConfiguration);
        }

        public IYoutubeImpl getYoutube()
        {
            return youtubeImpl;
        }
    }
}
