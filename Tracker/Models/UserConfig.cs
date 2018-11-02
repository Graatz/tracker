using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tracker.Models
{
    public class UserConfig
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int TimeSpanDays { get; set; }
        public int TimeSpanHours { get; set; }
        public int TimeSpanMinutes { get; set; }
        public double SearchingDistance { get; set; }
    }
}