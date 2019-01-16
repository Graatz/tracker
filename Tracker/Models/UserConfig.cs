using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Tracker.Models
{
    public class UserConfig
    {
        public int Id { get; set; }

        [DisplayName("Użytkownik")]
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        [DisplayName("Dni przedziału czasowego")]
        public int TimeSpanDays { get; set; }

        [DisplayName("Godziny przedziału")]
        public int TimeSpanHours { get; set; }

        [DisplayName("Minuty przedziału czasowego")]
        public int TimeSpanMinutes { get; set; }

        [DisplayName("Dystans przeszukiwania")]
        public double SearchingDistance { get; set; }
    }
}