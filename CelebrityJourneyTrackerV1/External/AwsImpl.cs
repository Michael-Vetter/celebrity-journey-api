using CelebrityJourneyTrackerV1.Configurations;
using CelebrityJourneyTrackerV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace CelebrityJourneyTrackerV1.External
{
    public interface IAwsImpl
    {
        Task<T> getfile<T>(string bucket, string key);
        Task<GetObjectResponse> getfileObject(string bucket, string key);
        Task<PutObjectResponse> putfile<T>(T contents, string bucket, string key);
        Task<PutObjectResponse> putfile(string contents, string bucket, string key);
    }

    public class AwsImplLive : IAwsImpl
    {
        private readonly AwsConfiguration _awsConfiguration;

        public AwsImplLive(AwsConfiguration awsConfiguration)
        {
            _awsConfiguration = awsConfiguration;
        }
        public async Task<T> getfile<T>(string bucket, string key)
        {
            T contents;

            using (var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
            {
                var currentContentsReq = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = key
                };

                var currentContents = await client.GetObjectAsync(currentContentsReq);
                using (var reader = new StreamReader(currentContents.ResponseStream, Encoding.UTF8))
                {
                    var rawFileContents = reader.ReadToEnd();
                    contents = JsonConvert.DeserializeObject<T>(rawFileContents);
                }

            }
            return contents;
        }

        public async Task<GetObjectResponse> getfileObject(string bucket, string key)
        {
            using (var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
            {
                var currentContentsReq = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = key
                };

                var currentContents = await client.GetObjectAsync(currentContentsReq);

                return currentContents;
            }
        }

        public async Task<PutObjectResponse> putfile<T>(T contents, string bucket, string key)
        {
            using (var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = key,
                    ContentBody = JsonConvert.SerializeObject(contents, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })
                };
                return await client.PutObjectAsync(putRequest);
            }
        }

        public async Task<PutObjectResponse> putfile(string contents, string bucket, string key)
        {
            using (var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = key,
                    ContentBody = contents
                };
                return await client.PutObjectAsync(putRequest);
            }
        }
    }


    public class AwsImplMock : IAwsImpl
    {
        private readonly AwsConfiguration _awsConfiguration;

        public AwsImplMock(AwsConfiguration awsConfiguration)
        {
            _awsConfiguration = awsConfiguration;
        }
        public async Task<T> getfile<T>(string bucket, string key)
        {
            var fileNamePath = $"C:\\Users\\micha\\source\\repos\\CelebrityJourneyTrackerV1\\CelebrityJourneyTrackerV1\\MockData\\{key}";

            return JsonConvert.DeserializeObject<T>(System.IO.File.ReadAllText(fileNamePath));

        }

        public Task<GetObjectResponse> getfileObject(string bucket, string key)
        {
            throw new NotImplementedException();
        }

        public async Task<PutObjectResponse> putfile<T>(T contents, string bucket, string key)
        {
            using (StreamWriter outputFile = new StreamWriter($"C:\\Users\\micha\\source\\repos\\CelebrityJourneyTrackerV1\\CelebrityJourneyTrackerV1\\MockData\\{key}"))
            {
                    outputFile.WriteLine(JsonConvert.SerializeObject(contents, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }));
            }

            return null;
        }

        public Task<PutObjectResponse> putfile(string contents, string bucket, string key)
        {
            throw new NotImplementedException();
        }
    }
}
