using System;
using CelebrityJourneyTrackerV1.Configurations;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CelebrityJourneyTrackerV1.External
{
    public interface IAwsFactory
    {
        IAwsImpl getAws();
    }

    public class AwsFactory : IAwsFactory
    {
        private readonly AwsConfiguration _awsConfiguration;
        private readonly IAwsImpl awsImpl;

        public AwsFactory(IOptions<AwsConfiguration> youtubeConfiguration)
        {
            _awsConfiguration = youtubeConfiguration.Value;
            if (_awsConfiguration.Environment == "test")
                awsImpl = new AwsImplMock(_awsConfiguration);
            else
                awsImpl = new AwsImplLive(_awsConfiguration);
        }

        public IAwsImpl getAws()
        {
            return awsImpl;
        }
    }
}
