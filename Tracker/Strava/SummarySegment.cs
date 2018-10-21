using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tracker.Strava
{
    public class SummarySegment
    {
        [JsonProperty("Id")]
        public long Id { get; set; }
    }
}