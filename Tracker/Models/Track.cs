using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tracker.Models
{
    public class Track
    {
        public int Id { get; set; }
        public long ExternalId { get; set; }

        [Required]
        [MaxLength(255)]
        [DisplayName("Nazwa trasy")]
        public string Name { get; set; }

        [MaxLength(255)]
        [DisplayName("Opis trasy (opcjonalnie)")]
        public string Description { get; set; }

        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }

        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }

        public double Distance { get; set; }
        public double AvarageSpeed { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime UploadDate { get; set; }
        public DateTime TrackDate { get; set; }
    }

    public class TrackSearchModel
    {
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public string Location { get; set; }
        public string User { get; set; }
    }

    public class ListTracksViewModel
    {
        public int NumberOfTracks { get; set; }
        public List<Track> Tracks { get; set; }
        public int CurrentPage { get; set; }
        public int StartingIndex { get; set; }
        public int NumberOfTracksPerPage { get; set; }
    }

    public class UserTracksViewModel
    {
        public List<Track> Tracks { get; set; }
    }

    public class DetailsViewModel
    {
        public Track Track { get; set; }
        public List<TrackPoint> TrackPoints { get; set; }
        public List<Track> SimilarTracks { get; set; }
    }

    public class CompareViewModel
    {
        public Track Track1 { get; set; }
        public Track Track2 { get; set; }
        public List<List<TrackPoint>> Segments1 { get; set; }
        public List<List<TrackPoint>> Segments2 { get; set; }
    }

    public class ImportConfirmationViewModel
    {
        public List<Strava.DetailedActivity> DetailedActivities { get; set; }
    }
}