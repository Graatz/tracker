using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tracker.Strava
{
    public class SummaryActivity
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}