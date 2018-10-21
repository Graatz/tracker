using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Tracker.Models;

namespace Tracker.Helpers
{
    public class GpxParser
    {
        public GpxParser()
        {

        }

        public List<TrackPoint> LoadGPXTracks(Stream file)
        {
            XDocument gpxDoc = XDocument.Load(file);
            XNamespace gpx = XNamespace.Get("http://www.topografix.com/GPX/1/1");

            List<TrackPoint> points = new List<TrackPoint>();

            IEnumerable<XElement> collection = gpxDoc.Descendants(gpx + "trkpt") != null ?
                                               gpxDoc.Descendants(gpx + "trkpt") : 
                                               gpxDoc.Descendants(gpx + "wpt");

            foreach (var item in collection)
            {
                float lat = float.Parse(item.Attribute("lat").Value, CultureInfo.InvariantCulture);
                float lon = float.Parse(item.Attribute("lon").Value, CultureInfo.InvariantCulture);
                DateTime date = DateTime.Now;
                try
                {
                    date = DateTime.Parse(item.Element(gpx + "time").Value);
                }
                catch (Exception ex)
                {
                    date = DateTime.Now;
                }

                points.Add(new TrackPoint(lat, lon, date));
            }

            return points;
        }
    }
}