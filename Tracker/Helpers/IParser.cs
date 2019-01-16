using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker.Models;

namespace Tracker.Helpers
{
    public interface IParser
    {
        List<TrackPoint> Parse(Stream fileStream);
    }
}
