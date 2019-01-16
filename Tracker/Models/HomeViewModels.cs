using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tracker.Models;

namespace Tracker.Models
{
    public class IndexViewModel
    {
        public int UserTracksTotal { get; set; }
        public double UserDistanceTotal { get; set; }
        public int UsersTotal { get; set; }
        public int TracksTotal { get; set; }
        public int TrackPointsTotal { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
        public PaginationViewModel PaginationViewModel { get; set; }
    }
}