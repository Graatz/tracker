using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Tracker.Strava
{
    public class PolylineMap
    {
        [JsonProperty("polyline")]
        public string Polyline { get; set; }
    }
}