using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Tracker.Models;

namespace Tracker.Helpers
{
    public class GpxParser : IParser
    {
        protected XNamespace Namespace { get; set; }

        public GpxParser()
        {
            Namespace = XNamespace.Get("http://www.topografix.com/GPX/1/1");
        }

        public List<TrackPoint> Parse(Stream fileStream)
        {
            XDocument content = XDocument.Load(fileStream);

            HashSet<TrackPoint> points = new HashSet<TrackPoint>(new TrackPointComparer());

            IEnumerable<XElement> collection = content.Descendants(Namespace + "trkpt") ?? content.Descendants(Namespace + "wpt");

            foreach (var item in collection)
            {
                float lat = float.Parse(item.Attribute("lat").Value, CultureInfo.InvariantCulture);
                float lon = float.Parse(item.Attribute("lon").Value, CultureInfo.InvariantCulture);
                DateTime date = DateTime.Now;
                float elevation = 0;

                if (item.Element(Namespace + "time") != null)
                    date = DateTime.Parse(item.Element(Namespace + "time").Value);

                if (item.Element(Namespace + "ele") != null)
                {
                    string el = item.Element(Namespace + "ele").Value.Replace('.', ',');
                    elevation = float.Parse(el);
                }

                points.Add(new TrackPoint(lat, lon, elevation, date));
            }

            return new List<TrackPoint>(points);
        }
    }
}