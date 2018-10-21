using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tracker.Models;

namespace Tracker.Models
{
    public class TrackPoint
    {
        public int Id { get; set; }

        public int Index { get; set; }

        public Track Track { get; set; }
        public int TrackId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime Date { get; set; }

        public TrackPoint()
        {

        }

        public TrackPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public TrackPoint(double latitude, double longitude, DateTime date)
        {
            Latitude = latitude;
            Longitude = longitude;

            Date = date;
        }

        public TrackPoint(double latitude, double longitude, DateTime date, int index)
        {
            Latitude = latitude;
            Longitude = longitude;

            Date = date;
            Index = index;
        }
    }
}