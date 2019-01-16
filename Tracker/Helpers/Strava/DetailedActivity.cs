using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tracker.Strava
{
    public class DetailedActivity
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("map")]
        public PolylineMap Map { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("distance")]
        public float Distance { get; set; }

        [JsonProperty("average_speed")]
        public float AverageSpeed { get; set; }

        [JsonProperty("start_date")]
        public DateTime Date { get; set; }

        public bool Import { get; set; }
    }
}