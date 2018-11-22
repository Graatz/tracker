using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tracker.Models;

namespace Tracker.Models
{
    public class IndexViewModel
    {
        public int UsersTotal { get; set; }
        public int TracksTotal { get; set; }
        public int TrackPointsTotal { get; set; }

        public int NumberOfTracks { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
        public int CurrentPage { get; set; }
        public int StartingIndex { get; set; }
        public int NumberOfTracksPerPage { get; set; }
    }
}