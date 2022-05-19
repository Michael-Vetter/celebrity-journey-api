using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CelebrityJourneyTrackerV1.Models
{
    public class LambdaRequestObject
    {
        RequestContext RequestContext { get; set; }
    }

    public class RequestContext
    {
        Identity Identity { get; set; }
    }

    public class Identity
    {
        string SourceIp { get; set; }
    }
}
