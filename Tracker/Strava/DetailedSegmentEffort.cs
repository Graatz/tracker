using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tracker.Strava
{
    public class DetailedSegmentEffort
    {
        [JsonProperty("segment")]
        public SummarySegment Segment { get; set; }
    }
}