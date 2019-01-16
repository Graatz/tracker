using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker.Models;
using Tracker.Strava;

namespace Tracker.Services
{
    public interface IStravaTrackService
    {
        bool AddTrack(DetailedActivity detailedActivity);
        Track GetTrackByExternalId(int trackId);
        string Authorize(string code);
        List<DetailedActivity> ImportActivities();
    }
}
