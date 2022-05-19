using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Amazon.S3;
using Amazon.S3.Model;
using CelebrityJourneyTrackerV1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using CelebrityJourneyTrackerV1.Services;
using Microsoft.AspNetCore.Http;

namespace CelebrityJourneyTrackerV1.Controllers
{

    public class ActivityController : ControllerBase
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

        private readonly string DATA_BUCKET = "celebrityjourneydatafiles";
        private readonly string LOG_BUCKET = "celebrityjourneyactivitylog";
        private readonly string FILE_DL = "dualipa.json";
        private readonly string FILE_DLV = "dualipavideo.json";
        private readonly string FILE_ADMINS = "admins.json";

        private readonly IYoutubeApi _youtubeApi;
        private readonly IAwsApi _awsApi;

        public ActivityController(IYoutubeApi youtubeApi,
            IAwsApi awsApi)
        {
            _youtubeApi = youtubeApi;
            _awsApi = awsApi;
        }

        #region GetPreSignedUrl
        [HttpGet]
        [Route("api/uploadurl/{fileName}")]
        public async Task<IActionResult> GetPreSignedUrl(string fileName, 
                [FromQuery] string fileType, 
                [FromQuery] string fileDate, 
                [FromQuery] string source,
                [FromQuery] string account,
                [FromQuery] string comment)
        {
            await LogEntry($"GetPreSignedUrl fileName({fileName}) account({account})", getIp(this.HttpContext));

            var imageMasterUpdated = await UpdateImageFile(fileDate, fileName, source, account, comment);

            if (!imageMasterUpdated)
                return BadRequest("update image file failed");

            var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);

            //Console.WriteLine($"fileName({fileName}) fileType({fileType})");
            var preSignedUrlRequest = new GetPreSignedUrlRequest()
            {
                BucketName = "www.celebrity-journey.com",
                Key = "img/" + fileName,
                ContentType = fileType,
                Expires = DateTime.Now.AddMinutes(60),
                Verb = HttpVerb.PUT
            };
            
            var psUrl = client.GetPreSignedURL(preSignedUrlRequest);

            var presignedPostUrlResponse = new PresignedPostUrlResponse()
            {
                Url = psUrl,
                Fields = new Fields()
                {
                    Key = "img/" + fileName,
                    Acl = "",
                    Bucket = "www.celebrity-journey.com"
                },
                FilePath = "img/" + fileName
            };

            return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(presignedPostUrlResponse));
        }

        private async Task<Boolean> UpdateImageFile(string fileDate, string fileName, string source, string account, string comment)
        {
            try
            {
                var images = await _awsApi.getFile<List<ImageMaster>>(DATA_BUCKET, "imageMaster.json");

                var newImage = new ImageMaster { Date = fileDate, FileName = fileName, Source = source, Account = account, Comment = comment };
                images.Add(newImage);

                var result = await _awsApi.putfile<List<ImageMaster>>(images, DATA_BUCKET, "imageMaster.json");
                
                if (result.HttpStatusCode == HttpStatusCode.OK)
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Post Activity Log exception ({ex.Message}) stack({ex.StackTrace})");
            }
            return false;

        }
        #endregion

        #region AdminLogin
        [HttpPost]
        [Route("api/admin")]
        public async Task<IActionResult> AdminLogin([FromBody] Admin admin)
        {
            await LogEntry($"AdminLogin admin.Name({admin.Name})", getIp(this.HttpContext));
            try
            {
                if (admin == null || string.IsNullOrEmpty(admin.Name))
                    return BadRequest("no login ID found");

                var adminsStore = await _awsApi.getFile<List<Admin>>(DATA_BUCKET, FILE_ADMINS);

                if (adminsStore.Where(_ => _.Name.ToLower() == admin.Name.ToLower()).Any() && admin.Name == admin.Password)
                    return Ok("Successful login");
                else
                    return NotFound("login failed");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
                return BadRequest("application error");
            }
        }

        #endregion

        #region AddVideo
        [HttpPost]
        [Route("api/video")]
        public async Task<IActionResult> AddVideo([FromBody] DuaLipa video)
        {
            await LogEntry($"AddVideo video.VideoId({video.VideoId})", getIp(this.HttpContext));
            try
            {
                var videoId = video.VideoId;

                if (videoId == null || videoId.Length == 0)
                    return BadRequest("no video ID found");

                var dl = await _awsApi.getFile<List<DuaLipa>>(DATA_BUCKET, FILE_DL);

                if (dl.Where(_ => _.VideoId == videoId).Any())
                    return BadRequest("Video Id already exists");

                var youtube = await _youtubeApi.getYoutubeData(videoId);
                if (youtube == null)
                    return NotFound("Video ID not found in Youtube");

                video.Id = Guid.NewGuid().ToString();

                if (string.IsNullOrEmpty(video.DateEvent))
                    video.DateSort = youtube.Snippet.PublishedAt;
                else
                    video.DateSort = video.DateEvent;
                dl.Add(video);

                await _awsApi.putfile(dl, DATA_BUCKET, FILE_DL);

                var dlvids = await _awsApi.getFile<List<Youtube>>(DATA_BUCKET, FILE_DLV);

                dlvids.Add(youtube);
                await _awsApi.putfile(dlvids, DATA_BUCKET, FILE_DLV);

                return Ok("video added");
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        #endregion

        #region GetVideosWithFilterAndPage
        [HttpGet]
        [Route("api/videos")]
        public async Task<IActionResult> GetVideosWithFilterAndPage([FromQuery] int pageSize, 
            [FromQuery] int page, 
            [FromQuery] string categories, 
            [FromQuery] string songs)
        {
            await LogEntry($"GetVideosWithFilterAndPage categories({categories}) songs({songs})", getIp(this.HttpContext));
            Dictionary<string, Object> dayItems = new Dictionary<string, object>();

            var videoDetails = await _awsApi.getFile<List<DuaLipaVideo>>(DATA_BUCKET, "dualipavideo.json");
            var videos = await _awsApi.getFile<List<DuaLipa>>(DATA_BUCKET, FILE_DL);
            videos.Sort();

            if (categories != null && categories != string.Empty)
            {
                var searchCats = categories.Split(',',StringSplitOptions.RemoveEmptyEntries);
                videos = videos.Where(_ => searchCats.Contains(_.Category)).ToList();
            }

            if (songs != null && songs != string.Empty)
            {
                var searchSongs = songs.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(_ => new Song { Name = _.Trim() }).ToList();
                videos = videos.Where(_ => _.Songs.Select(c => c.Song).ToList().Intersect(searchSongs.Select(s => s.Name).ToList()).Any()).ToList();
            }

            List<DuaLipaVideo> videoDetailsResponse = new List<DuaLipaVideo>();
            foreach (var videoData in videos)
            {
                var dlVideo = videoDetails.Where(_ => _.Id == videoData.VideoId).FirstOrDefault();
                dlVideo.DateEvent = videoData.DateEvent;
                dlVideo.Category = videoData.Category;
                dlVideo.Songs = videoData.Songs;
                videoDetailsResponse.Add(dlVideo);
            }
            dayItems.Add("videos", videoDetailsResponse);
            dayItems.Add("total", videoDetailsResponse.Count);
            dayItems.Add("pageSize", videoDetailsResponse.Count);
            dayItems.Add("page", 1);

            return Ok(JsonConvert.SerializeObject(dayItems));
        }
        #endregion

        #region GetItemsForDate
        [HttpGet]
        [Route("api/calendar/dayitems/{date}")]
        public async Task<IActionResult> GetItemsForDate([FromRoute] string date)
        {
            await LogEntry($"GetItemsForDate date({date})", getIp(this.HttpContext));
            Dictionary<string, Object> dayItems = new Dictionary<string, object>();

            var videoDetails = await _awsApi.getFile<List<DuaLipaVideo>>(DATA_BUCKET, "dualipavideo.json");
            var videos = await _awsApi.getFile<List<DuaLipa>>(DATA_BUCKET, "dualipa.json");
            var images = await _awsApi.getFile<List<ImageMaster>>(DATA_BUCKET, "imageMaster.json");

            var videosForDate = videos.Where(_ => _.DateSort == date).ToList();

            List<DuaLipaVideo> videoDetailsResponse = new List<DuaLipaVideo>();
            foreach (var videoData in videosForDate)
            {
                var dlVideo = videoDetails.Where(_ => _.Id == videoData.VideoId).FirstOrDefault();
                dlVideo.DateEvent = videoData.DateEvent;
                videoDetailsResponse.Add(dlVideo);
            }
            dayItems.Add("videos", videoDetailsResponse);

            List<ImageMaster> imagesForDate = images.Where(_ => _.Date == date).ToList();
            dayItems.Add("images", imagesForDate);

            return Ok(JsonConvert.SerializeObject(dayItems));
        }
        #endregion

        #region GetCalendarMonth
        [HttpGet]
        [Route("api/calendar/month/{year}/{month}")]
        public async Task<IActionResult> GetCalendarMonth([FromRoute] string year, [FromRoute] string month)
        {
            await LogEntry($"GetCalendarMonth year({year}) month({month})", getIp(this.HttpContext));
            var years = await _awsApi.getFile<List<EventYear>>(DATA_BUCKET, "events.json");
            var videos = await _awsApi.getFile<List<DuaLipa>>(DATA_BUCKET, "dualipa.json");
            var images = await _awsApi.getFile<List<ImageMaster>>(DATA_BUCKET, "imageMaster.json");

            var step1 = years.Where(_ => _.Year.ToString() == year).FirstOrDefault();
            if (step1 == null)
                return NotFound("step 1");

            var step2 = step1.Months.Where(_ => _.Month == month).FirstOrDefault();
            if (step2 == null)
                return NotFound("step 2");

            var monthEvents = step2.Events.ToList();
            if (monthEvents == null)
                return NotFound("monthEvents");

            var reponseEvents = new List<DayEvent>();
            foreach (var dayEvent in monthEvents)
            {
                var videoCount = videos.Where(_ => _.DateSort == dayEvent.DateString).Count();
                var imageCount = images.Where(_ => _.Date == dayEvent.DateString).Count();
                var dayEventResponse = new DayEvent { Title = $"{dayEvent.Name} (Videos: {videoCount} Images: {imageCount})", Date = dayEvent.Date.ToString("yyyy-MM-dd") };
                reponseEvents.Add(dayEventResponse);
            }

            var response = new CalendarMonthResponse();
            response.Events = reponseEvents;

            return Ok(JsonConvert.SerializeObject(response));
        }
        #endregion

        #region GetCalendarNav
        [HttpGet]
        [Route("api/calendar/nav")]
        public async Task<IActionResult> GetCalendarNav()
        {
            await LogEntry($"GetCalendarNav", getIp(this.HttpContext));
            var years = await _awsApi.getFile<List<EventYear>>(DATA_BUCKET, "events.json");
            
            int index = 0;
            var main = new Dictionary<string, CalendarNavItem>();

            var yearList = years.Select(_ => _.Year).OrderBy(y => y).ToList();

            foreach( var year in yearList)
            {
                var mainIndex = index;
                index++;

                var nodesForYear = new Dictionary<string, CalendarNavItem>();
                var months = years.Where(_ => _.Year == year).SelectMany(y => y.Months).Select(m => m.Month).ToList();
                foreach(var month in months)
                {
                    var monthItem = new CalendarNavItem { Label = month, Index = index, Nodes = new Dictionary<string, CalendarNavItem>() };
                    nodesForYear.Add(month, monthItem);
                    index++;
                }
                main.Add(year.ToString(), new CalendarNavItem { Label = year.ToString(), Index = mainIndex, Nodes = nodesForYear });
            }

            return Ok(JsonConvert.SerializeObject(main));
        }
        #endregion

        #region GetEventsYearMonth
        [HttpGet]
        [Route("api/events/{eventYear}/{eventMonth}")]
        public async Task<IActionResult> GetEventsYearMonth([FromRoute] int eventYear, [FromRoute] int eventMonth)
        {
            await LogEntry($"GetEventsYearMonth eventYear({eventYear}) eventMonth({eventMonth})", getIp(this.HttpContext));

            if (eventMonth < 1 || eventMonth > 12)
                return BadRequest("event month invalid");

            if( eventYear < 2014 || eventYear > 2030)
                return BadRequest("event year invalid");

            var monthString = monthText[eventMonth - 1];

            var years = await _awsApi.getFile<List<EventYear>>(DATA_BUCKET, "events.json");
            
            var events = years.Where(_ => _.Year == eventYear).SelectMany(x => x.Months).Where(m => m.Month == monthString).ToList();

            if (events.Count == 0)
                return NotFound("no data matching criteria");

            return Ok(JsonConvert.SerializeObject(events));
        }
        #endregion

        #region GetEvents
        [HttpGet]
        [Route("api/events")]
        public async Task<IActionResult> GetEvents()
        {
            await LogEntry($"GetEvents", getIp(this.HttpContext));
            var years = await _awsApi.getFile<List<EventYear>>(DATA_BUCKET, "events.json");

            return Ok(JsonConvert.SerializeObject(years));
        }
        #endregion

        #region AddEvent
        [HttpPost]
        [Route("api/events")]
        public async Task<IActionResult> AddEvent([FromBody] Event newEvent)
        {
            await LogEntry($"AddEvent newEvent.DateString({newEvent.DateString})", getIp(this.HttpContext));
            var newEventDate = newEvent.DateString;
            var newEventMonthString = newEvent.Date.ToString("MMMM");
            var newEventYear = int.Parse(newEvent.Date.ToString("yyyy"));

            //Console.WriteLine($"AddEvent newEventDate({newEventDate}) newEventMonthString({newEventMonthString}) newEventYear({newEventYear})");

            var allEvents = await _awsApi.getFile<List<EventYear>>(DATA_BUCKET, "events.json");

            if (allEvents.Where(_ => _.Year == newEventYear).ToList().Count == 0)
            {
                allEvents.Add(new EventYear { Year = newEventYear, Months = new List<EventMonth>() });
                allEvents.Sort();
            }

            //get months for year
            var monthsForTarget = allEvents.Where(_ => _.Year == newEventYear).First().Months.Where(_ => _.Month == newEventMonthString).ToList();
            if (monthsForTarget.Count == 0)
            {
                allEvents.Where(_ => _.Year == newEventYear).ToList()[0]
                    .Months.Add(new EventMonth { Month = newEventMonthString, Events = new List<Event>() });
                allEvents.Where(_ => _.Year == newEventYear).ToList()[0]
                    .Months.Sort();
            }

            //get events for month
            //add or update event
            var eventsForTarget = allEvents.Where(_ => _.Year == newEventYear).First().Months.Where(_ => _.Month == newEventMonthString).First().Events.ToList();
            if (eventsForTarget.Where(_ => _.DateString == newEventDate).ToList().Count == 0)
            {
                allEvents.Where(_ => _.Year == newEventYear).ToList()[0]
                    .Months.Where(_ => _.Month == newEventMonthString).ToList()[0]
                    .Events.Add(newEvent);
                allEvents.Where(_ => _.Year == newEventYear).ToList()[0]
                    .Months.Where(_ => _.Month == newEventMonthString).ToList()[0]
                    .Events.Sort();
            }
            else
            {
                var eventCopy = allEvents.Where(_ => _.Year == newEventYear).ToList()[0]
                    .Months.Where(_ => _.Month == newEventMonthString).ToList()[0]
                    .Events.Where(_ => _.DateString == newEventDate).ToList()[0];
                eventCopy.Name = newEvent.Name;
                eventCopy.Org = newEvent.Org;
                eventCopy.Locality = newEvent.Locality;
                eventCopy.Region = newEvent.Region;
                eventCopy.CountryName = newEvent.CountryName;
            }

            using (var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = DATA_BUCKET,
                    Key = "events.json",
                    ContentBody = JsonConvert.SerializeObject(allEvents, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    })
                };
                await client.PutObjectAsync(putRequest);
            }

            return Ok(JsonConvert.SerializeObject(allEvents, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
        }

        #endregion


        #region UpdateAllVideos
        [HttpGet]
        [Route("api/updateallvideos")]
        public async Task<IActionResult> UpdateAllVideos()
        {
            await LogEntry($"UpdateAllVideos", getIp(this.HttpContext));
            List<Youtube> updatedVideos = new List<Youtube>();

            var videoDetails = await _awsApi.getFile<List<Youtube>>(DATA_BUCKET, FILE_DLV);

            foreach (var videoDetail in videoDetails)
            {
                var youtube = await _youtubeApi.getYoutubeData(videoDetail.Id);
                if (youtube == null)
                    Console.WriteLine($"Video ID removed ({videoDetail.Id})");
                else
                {
                    updatedVideos.Add(youtube);
                }
            }
            await _awsApi.putfile(updatedVideos, DATA_BUCKET, FILE_DLV);

            return Ok(JsonConvert.SerializeObject(updatedVideos));
        }
        #endregion

        #region activity-log
        [HttpPost]
        [Route("api/activity/log")]
        public async Task<PutObjectResponse> Post([FromBody] LogData logData)
        {
            return await LogEntry(logData.Data);
            
        }
        #endregion

        #region GET activity-log
        [HttpGet]
        [Route("api/activity/log")]
        public async Task<IActionResult> GetActivityLog()
        {
            List<string> logReversed = new List<string>();
            
            var currentContents = await _awsApi.getfileObject(LOG_BUCKET, "activityLog");
            var currentContentsStream = currentContents.ResponseStream;
            
            var reader = new StreamReader(currentContentsStream);
            {
                while(!reader.EndOfStream)
                    logReversed.Add(reader.ReadLine());

            };

            logReversed.Reverse();
            return Ok(logReversed);
        }
        #endregion

        private string getIp(HttpContext httpContext)
        {
            try
            {
                //can't seem to get the httpContext items into an object, so gave up trying for now.
                var httpContextItems = JsonConvert.SerializeObject(httpContext.Items["LambdaRequestObject"]);
                var sourceIpStart = httpContextItems.IndexOf("SourceIp");
                var sourceIpEnd = httpContextItems.IndexOf("\",", sourceIpStart);
                var sourceIp = httpContextItems.Substring(sourceIpStart + 11, sourceIpEnd - sourceIpStart - 11);
                //Console.WriteLine($"get SourceIP sourceIpStart({sourceIpStart}) sourceIpEnd({sourceIpEnd}) sourceIp({sourceIp})");
                return sourceIp;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LogEntry exception ({ex.Message}) stack({ex.StackTrace})");
                return "IP Not Found";
            }
        }

        private async Task<PutObjectResponse> LogEntry(string data, string IpAddress = "")
        {
            try
            {
                var currentContents = await _awsApi.getfileObject(LOG_BUCKET, "activityLog");
                var currentContentsStream = currentContents.ResponseStream;
                DateTime currentDT = DateTime.Now;
                var currentDateEST = currentDT.AddHours(-4);
                string responseBody = "";
                var reader = new StreamReader(currentContentsStream);
                {
                    responseBody = reader.ReadToEnd() + Environment.NewLine;
                    var newLog = string.Concat(responseBody, currentDateEST.ToString("yyyy-MM-dd hh:mm:ss tt") + " " + IpAddress + ", " + data);

                    return await _awsApi.putfile(newLog, LOG_BUCKET, "activityLog");
                };
            }
            catch(Exception ex)
            {
                Console.WriteLine($"LogEntry exception ({ex.Message}) stack({ex.StackTrace})");
            }
            return null;

        }

    }

    public class LogData
    {
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
