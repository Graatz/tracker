using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tracker.Strava
{
    public class Athlete
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("sex")]
        public string Sex { get; set; }

        [JsonProperty("premium")]
        public bool Premium { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("badge_type_id")]
        public int BadgeTypeId { get; set; }

        [JsonProperty("profile_medium")]
        public string ProfileMedium { get; set; }

        [JsonProperty("profile")]
        public string Profile { get; set; }

        [JsonProperty("friend")]
        public string Friend { get; set; }

        [JsonProperty("follower")]
        public string Follower { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}