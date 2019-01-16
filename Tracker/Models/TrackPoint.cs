using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Tracker.Models;

namespace Tracker.Models
{
    public class TrackPoint
    {
        public int Id { get; set; }

        public int Index { get; set; }

        [DisplayName("Trasa")]
        public Track Track { get; set; }
        public int TrackId { get; set; }

        [Index("LatitudeIndex")]
        [DisplayName("Szerokość geograficzna")]
        public double Latitude { get; set; }

        [Index("LongitudeIndex")]
        [DisplayName("Długość geograficzna")]
        public double Longitude { get; set; }

        [DisplayName("Wysokość")]
        public float Elevation { get; set; }

        [DisplayName("Data")]
        public DateTime? Date { get; set; }

        public TrackPoint()
        {

        }

        public TrackPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public TrackPoint(double latitude, double longitude, float elevation, DateTime date)
        {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;

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

    public class TrackPointComparer : IEqualityComparer<TrackPoint>
    {
        public bool Equals(TrackPoint x, TrackPoint y)
        {
            return x.Latitude.Equals(y.Latitude) && x.Longitude.Equals(y.Longitude);
        }

        public int GetHashCode(TrackPoint obj)
        {
            return obj.Latitude.GetHashCode() ^ obj.Longitude.GetHashCode();
        }
    }
}