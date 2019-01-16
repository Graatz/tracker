using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tracker.Models;

namespace Tracker.DTO
{
    public class TrackDTO
    {
        public int Id { get; set; }
        public long ExternalId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Distance { get; set; }

        public double AvarageSpeed { get; set; }

        public string StartLocation { get; set; }

        public string EndLocation { get; set; }

        public DateTime UploadDate { get; set; }

        public DateTime? TrackDate { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }

        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }
    }
}