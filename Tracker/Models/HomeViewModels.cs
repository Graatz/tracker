using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tracker.Models;

namespace Tracker.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Track> Tracks { get; set; }
        public int NumberOfUsers { get; set; }
        public int NumberOfTracks { get; set; }
        public int NumberOfTrackPoints { get; set; }
    }
}