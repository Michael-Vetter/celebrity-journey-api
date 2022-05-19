using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.S3.Model;
using CelebrityJourneyTrackerV1.External;
using CelebrityJourneyTrackerV1.Models;

namespace CelebrityJourneyTrackerV1.Services
{
    public interface IAwsApi
    {
        Task<T> getFile<T>(string bucket, string key);
        Task<GetObjectResponse> getfileObject(string bucket, string key);
        Task<PutObjectResponse> putfile<T>(T contents, string bucket, string key);
        Task<PutObjectResponse> putfile(string contents, string bucket, string key);
    }

    public class AwsApi : IAwsApi
    {
        private readonly IAwsImpl _awsImpl;

        public AwsApi(IAwsFactory awsFactory)
        {
            _awsImpl = awsFactory.getAws();
        }

        public async Task<T> getFile<T>(string bucket, string key)
        {
            return await _awsImpl.getfile<T>(bucket, key);
        }

        public async Task<GetObjectResponse> getfileObject(string bucket, string key)
        {
            return await _awsImpl.getfileObject(bucket, key);
        }

        public async Task<PutObjectResponse> putfile<T>(T contents, string bucket, string key)
        {
            return await _awsImpl.putfile(contents, bucket, key);
        }

        public async Task<PutObjectResponse> putfile(string contents, string bucket, string key)
        {
            return await _awsImpl.putfile(contents, bucket, key);
        }
    }
}
