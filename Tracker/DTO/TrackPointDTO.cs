using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tracker.DTO
{
    public class TrackPointDTO
    {
        public int Id { get; set; }

        public int Index { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public float Elevation { get; set; }

        public DateTime? Date { get; set; }
    }
}