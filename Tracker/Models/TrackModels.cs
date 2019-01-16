using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tracker.DTO;

namespace Tracker.Models
{
    public class Track
    {
        public int Id { get; set; }
        public long ExternalId { get; set; }

        [DisplayName("Zewnętrzna sygnatura")]
        public string ExternalSignature { get; set; }

        [Required]
        [MaxLength(255)]
        [DisplayName("Nazwa trasy")]
        public string Name { get; set; }

        [MaxLength(255)]
        [DisplayName("Opis trasy (opcjonalnie)")]
        public string Description { get; set; }

        [DisplayName("Dystans")]
        public double Distance { get; set; }

        [DisplayName("Średnia prędkość")]
        public double AvarageSpeed { get; set; }

        [DisplayName("Lokacja startowa")]
        public string StartLocation { get; set; }

        [DisplayName("Lokacja końcowa")]
        public string EndLocation { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [DisplayName("Data przesłania")]
        public DateTime UploadDate { get; set; }

        [DisplayName("Data trasy")]
        public DateTime? TrackDate { get; set; }

        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }

        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }

        public ICollection<TrackPoint> TrackPoints { get; set; }
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
        public List<Track> Tracks { get; set; }
        public PaginationViewModel PaginationViewModel { get; set; }
    }

    public class DetailsViewModel
    {
        public TrackDTO Track { get; set; }
        public List<TrackPointDTO> TrackPoints { get; set; }
    }

    public class SimilarTracksViewModel
    {
        public Track Track { get; set; }
        public List<Track> SimilarTracks { get; set; }
    }

    public class CompareViewModel
    {
        public TrackDTO Track1 { get; set; }
        public TrackDTO Track2 { get; set; }
        public List<TrackPointDTO> Track2TrackPoints { get; set; }
        public List<List<TrackPointDTO>> Segments1 { get; set; }
        public List<TrackSegment> Segments2 { get; set; }
    }

    public class ImportConfirmationViewModel
    {
        public List<Strava.DetailedActivity> DetailedActivities { get; set; }
    }

    public class TrackSegment
    {
        public List<TrackPointDTO> TrackPoints { get; set; }
        public double Distance { get; set; }
        public double AvarageSpeed { get; set; }
    }

    public class PaginationViewModel
    {
        public TrackSearchModel SearchModel { get; set; }
        public string ViewName { get; set; }
        public int NumberOfTracks { get; set; }
        public int StartingIndex { get; set; }
        public int NumberOfTracksPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int DisplayFrom { get; set; }
        public int DisplayTo { get; set; }

        public PaginationViewModel(string viewName, TrackSearchModel searchModel, int numberOfTracks, int startingIndex, int numberOfTracksPerPage, int currentPage)
        {
            SearchModel = searchModel;
            ViewName = viewName;
            NumberOfTracks = numberOfTracks;
            StartingIndex = startingIndex;
            NumberOfTracksPerPage = numberOfTracksPerPage;
            CurrentPage = currentPage;

            if (currentPage > 0)
                DisplayFrom = currentPage - 1;

            if (currentPage < Math.Ceiling((double)((double)numberOfTracks / (double)numberOfTracksPerPage)) - 1)
                DisplayTo = currentPage + 1;
            else
                DisplayTo = (int)Math.Ceiling((double)((double)numberOfTracks / (double)numberOfTracksPerPage)) - 1;
        }
    }
}